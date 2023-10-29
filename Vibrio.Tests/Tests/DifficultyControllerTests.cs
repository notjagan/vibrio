using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Net;
using System.Text.Json;
using System.Web;
using Vibrio.Controllers;
using Vibrio.Tests.Utilities;
using Xunit.Abstractions;

namespace Vibrio.Tests.Tests {
    public class DifficultyControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>, IDisposable {
        private readonly HttpClient client;

        public DifficultyControllerTests(WebApplicationFactory<Startup> factory) {
            client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
        }

        public async void Dispose() {
            await client.DeleteAsync("api/cache");
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<object[]> RawBeatmapTestData =>
            DifficultyControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Data, beatmap.Mods, beatmap.StarRating, beatmap.MaxCombo }
            );

        public static IEnumerable<object[]> IdTestData =>
            DifficultyControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Id, beatmap.Mods, beatmap.StarRating, beatmap.MaxCombo }
            );

        [Theory]
        [MemberData(nameof(RawBeatmapTestData))]
        public void Get_difficulty_attributes(byte[] beatmapData, Mod[] mods, float starRating, int maxCombo) {
            var beatmap = beatmapData.LoadBeatmap();

            var attributes = DifficultyController.GetDifficulty(beatmap, mods);

            Assert.InRange(attributes.StarRating, starRating - 0.03, starRating + 0.03);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }

        [Theory]
        [MemberData(nameof(IdTestData))]
        public async Task Get_difficulty_attributes_from_endpoint(int beatmapId, Mod[] mods, float starRating, int maxCombo) {
            var attributes = await RequestUtilities.RequestDifficulty(client, beatmapId, mods);

            Assert.NotNull(attributes);
            Assert.Equal(mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, starRating - 0.03, starRating + 0.03);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }

        [Theory]
        [MemberData(nameof(RawBeatmapTestData))]
        public async void Get_difficulty_attributes_from_endpoint_through_file_upload(byte[] beatmapData, Mod[] mods, float starRating, int maxCombo) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/difficulty"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var mod in mods) {
                query.Add("mods", mod.Acronym);
            }
            builder.Query = query.ToString();

            var stream = new MemoryStream(beatmapData);
            var file = new StreamContent(stream);
            var content = new MultipartFormDataContent { { file, "file", "file" } };

            var response = await client.PostAsync(builder.Uri, content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuDifficultyAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.Equal(mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, starRating - 0.03, starRating + 0.03);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }
    }
}