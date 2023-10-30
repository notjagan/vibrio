using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Web;
using Vibrio.Models;

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
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(body, RequestUtilities.SerializerOptions);
        }

        public static void AddObject(this NameValueCollection query, object obj, Func<object, string> serialize) {
            var properties = obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties) {
                var value = property.GetValue(obj, null)!;
                if (value is object[] array) {
                    foreach (var item in array) {
                        query.Add(property.Name, serialize(item));
                    }
                } else {
                    query.Add(property.Name, serialize(value));
                }
            }
        }

        public static MultipartFormDataContent ToFormContent(this byte[] data, string fileName) {
            var stream = new MemoryStream(data);
            var file = new StreamContent(stream);
            return new MultipartFormDataContent { { file, fileName, fileName } };
        }
    }
}
