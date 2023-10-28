using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Text.Json;
using System.Web;
using vibrio.src;
using vibrio.src.Models;
using Vibrio.Tests.Utilities;
using Xunit.Abstractions;

namespace Vibrio.Tests.Tests {
    public class PerformanceControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>, IDisposable {
        private readonly HttpClient client;
        private readonly JsonSerializerOptions serializerOptions;

        public PerformanceControllerTests(WebApplicationFactory<Startup> factory) {
            client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
            serializerOptions = new JsonSerializerOptions() {
                PropertyNameCaseInsensitive = true,
            };
            serializerOptions.Converters.Add(new ModConverter());
        }

        public async void Dispose() {
            await client.DeleteAsync("api/cache");
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        public static IEnumerable<object[]> PerformanceTestData =>
            PerformanceControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Id, beatmap.Info, beatmap.Pp }
            );

        [Theory]
        [MemberData(nameof(PerformanceTestData))]
        public async Task Verify_performance_endpoint_with_beatmap(int beatmapId, BasicScoreInfo info, double pp) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance/{beatmapId}"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(info, value => {
                if (value is Mod mod) {
                    return mod.Acronym;
                } else {
                    return value.ToString()!;
                }
            });
            builder.Query = query.ToString();

            var response = await client.GetAsync(builder.Uri);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, serializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, pp - 0.05, pp + 0.05);
        }
    }
}