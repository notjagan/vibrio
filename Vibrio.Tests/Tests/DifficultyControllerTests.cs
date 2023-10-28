using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Mods;
using System.Text.Json;
using System.Web;
using vibrio.Controllers;
using vibrio.src;
using vibrio.src.Models;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Tests {
    public class DifficultyControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>, IDisposable {
        private readonly HttpClient client;
        private readonly JsonSerializerOptions serializerOptions;

        public DifficultyControllerTests(WebApplicationFactory<Startup> factory) {
            client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
            serializerOptions = new JsonSerializerOptions {
                PropertyNameCaseInsensitive = true,
                Converters = { new ModConverter() }
            };
        }

        public async void Dispose() {
            await client.DeleteAsync("api/cache");
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<object[]> ControllerTestData =>
            DifficultyControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Data, beatmap.Mods, beatmap.StarRating, beatmap.MaxCombo }
            );

        public static IEnumerable<object[]> EndpointTestData =>
            DifficultyControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Id, beatmap.Mods, beatmap.StarRating, beatmap.MaxCombo }
            );

        [Theory]
        [MemberData(nameof(ControllerTestData))]
        public void Verify_difficulty_attributes(byte[] beatmapData, Mod[] mods, float starRating, int maxCombo) {
            JsonSerializer.Serialize(new OsuModHidden(), serializerOptions);
            var beatmap = beatmapData.LoadBeatmap();

            var attributes = DifficultyController.GetDifficulty(beatmap, mods);

            Assert.InRange(attributes.StarRating, starRating - 0.01, starRating + 0.01);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }

        public async Task<OsuDifficultyAttributes?> RequestDifficulty(HttpClient client, int beatmapId, Mod[] mods) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/difficulty/{beatmapId}"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var mod in mods) {
                query.Add("mods", mod.Acronym);
            }
            builder.Query = query.ToString();

            var response = await client.GetAsync(builder.Uri);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(body, serializerOptions);
        }

        [Theory]
        [MemberData(nameof(EndpointTestData))]
        public async Task Verify_difficulty_endpoint_results(int beatmapId, Mod[] mods, float starRating, int maxCombo) {
            var attributes = await RequestDifficulty(client, beatmapId, mods);

            Assert.NotNull(attributes);
            Assert.Equal(mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, starRating - 0.03, starRating + 0.03);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }
    }
}