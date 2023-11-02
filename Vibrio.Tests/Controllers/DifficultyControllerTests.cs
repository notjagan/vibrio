using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Net;
using System.Text.Json;
using System.Web;
using Vibrio.Controllers;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Controllers {
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

        // wrapper since MemberData doesn't seem to work with definitions in other classes
        public static IEnumerable<object[]> TestData => DifficultyControllerTestData.TestData.Select(data => new object[] { data });

        [Theory]
        [MemberData(nameof(TestData))]
        public void Get_difficulty_attributes(DifficultyControllerTestData.TestBeatmap data) {
            var beatmap = data.Data.LoadBeatmap();

            var attributes = DifficultyController.GetDifficulty(beatmap, data.Mods);

            Assert.InRange(attributes.StarRating, data.StarRating - 0.03, data.StarRating + 0.03);
            Assert.Equal(data.MaxCombo, attributes.MaxCombo);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async Task Get_difficulty_attributes_from_endpoint(DifficultyControllerTestData.TestBeatmap data) {
            var attributes = await RequestUtilities.RequestDifficulty(client, data.Id, data.Mods);

            Assert.NotNull(attributes);
            Assert.Equal(data.Mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, data.StarRating - 0.03, data.StarRating + 0.03);
            Assert.Equal(data.MaxCombo, attributes.MaxCombo);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public async void Get_difficulty_attributes_from_endpoint_uploaded_file(DifficultyControllerTestData.TestBeatmap data) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/difficulty"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var mod in data.Mods) {
                query.Add("mods", mod.Acronym);
            }
            builder.Query = query.ToString();

            var response = await client.PostAsync(builder.Uri, data.Data.ToFormContent("beatmap"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuDifficultyAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.Equal(data.Mods.Select(mod => mod.Acronym), attributes!.Mods.Select(mod => mod.Acronym));
            Assert.InRange(attributes.StarRating, data.StarRating - 0.03, data.StarRating + 0.03);
            Assert.Equal(data.MaxCombo, attributes.MaxCombo);
        }
    }
}
