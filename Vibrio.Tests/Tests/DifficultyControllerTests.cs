using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using Vibrio.Controllers;
using Vibrio.Tests.Utilities;

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
            var beatmap = beatmapData.LoadBeatmap();

            var attributes = DifficultyController.GetDifficulty(beatmap, mods);

            Assert.InRange(attributes.StarRating, starRating - 0.01, starRating + 0.01);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }

        [Theory]
        [MemberData(nameof(EndpointTestData))]
        public async Task Verify_difficulty_endpoint_results(int beatmapId, Mod[] mods, float starRating, int maxCombo) {
            var attributes = await RequestUtilities.RequestDifficulty(client, beatmapId, mods);

            Assert.NotNull(attributes);
            Assert.Equal(mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, starRating - 0.03, starRating + 0.03);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }
    }
}