using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Text.Json;
using System.Web;
using vibrio.src.Models;

namespace Vibrio.Tests.Utilities {
    public static class RequestUtilities {
        public static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true,
            Converters = { new ModConverter() }
        };

        public static async Task<OsuDifficultyAttributes?> RequestDifficulty(HttpClient client, int beatmapId, Mod[] mods) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/difficulty/{beatmapId}"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var mod in mods) {
                query.Add("mods", mod.Acronym);
            }
            builder.Query = query.ToString();

            var response = await client.GetAsync(builder.Uri);
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(body, RequestUtilities.SerializerOptions);
        }
    }
}
