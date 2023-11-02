using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Net;
using System.Text.Json;
using System.Web;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Controllers {
    public class PerformanceControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>, IDisposable {
        private readonly HttpClient client;

        public PerformanceControllerTests(WebApplicationFactory<Startup> factory) {
            client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
        }

        public async void Dispose() {
            await client.DeleteAsync("api/cache");
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        // wrapper since MemberData doesn't seem to work with definitions in other classes
        public static IEnumerable<object[]> TestData => PerformanceControllerTestData.TestData.Select(data => new object[] { data });

        private static string SerializeScoreProperty(object value) {
            if (value is Mod mod) {
                return mod.Acronym;
            } else {
                return value.ToString()!;
            }
        }

        private async Task Get_performance_attributes(UriBuilder builder, double pp) {
            var response = await client.GetAsync(builder.Uri);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, pp - 0.05, pp + 0.05);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_performance_attributes_from_beatmap_id(PerformanceControllerTestData.TestBeatmap data) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance/{data.Id}"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(data.Info, SerializeScoreProperty);
            builder.Query = query.ToString();

            await Get_performance_attributes(builder, data.Pp);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_performance_attributes_from_difficulty(PerformanceControllerTestData.TestBeatmap data) {
            var difficultyAttributes = await RequestUtilities.RequestDifficulty(client, data.Id, data.Info.Mods);
            // avoid redundant mods in query string from score info
            difficultyAttributes!.Mods = Array.Empty<Mod>();
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(difficultyAttributes, SerializeScoreProperty);
            query.AddObject(data.Info, SerializeScoreProperty);
            builder.Query = query.ToString();

            await Get_performance_attributes(builder, data.Pp);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_performance_attributes_from_uploaded_file(PerformanceControllerTestData.TestBeatmap data) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(data.Info, SerializeScoreProperty);
            builder.Query = query.ToString();

            var response = await client.PostAsync(builder.Uri, data.BeatmapData.ToFormContent("beatmap"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, data.Pp - 0.05, data.Pp + 0.05);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_performance_attributes_from_uploaded_replay(PerformanceControllerTestData.TestBeatmap data) {
            var response = await client.PostAsync($"api/performance/replay/{data.Id}", data.ReplayData.ToFormContent("replay"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, data.Pp - 0.05, data.Pp + 0.05);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_performance_attributes_from_uploaded_replay_and_beatmap(PerformanceControllerTestData.TestBeatmap data) {
            using var beatmapStream = new MemoryStream(data.BeatmapData);
            using var beatmapFile = new StreamContent(beatmapStream);
            using var replayStream = new MemoryStream(data.ReplayData);
            using var replayFile = new StreamContent(replayStream);
            var content = new MultipartFormDataContent {
                { beatmapFile, "beatmap", "beatmap" },
                { replayFile, "replay", "replay" }
            };

            var response = await client.PostAsync($"api/performance/replay", content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, data.Pp - 0.05, data.Pp + 0.05);
        }
    }
}
