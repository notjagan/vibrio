﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Difficulty;
using System.Net;
using System.Text.Json;
using System.Web;
using Vibrio.Models;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Tests {
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

        public static IEnumerable<object[]> RawBeatmapTestData =>
            PerformanceControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Data, beatmap.Info, beatmap.Pp }
            );

        public static IEnumerable<object[]> IdTestData =>
            PerformanceControllerTestData.TestData.Select(beatmap =>
                new object[] { beatmap.Id, beatmap.Info, beatmap.Pp }
            );

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
        [MemberData(nameof(IdTestData))]
        public async Task Get_performance_attributes_from_beatmap_id(int beatmapId, BasicScoreInfo info, double pp) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance/{beatmapId}"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(info, SerializeScoreProperty);
            builder.Query = query.ToString();

            await Get_performance_attributes(builder, pp);
        }

        [Theory]
        [MemberData(nameof(IdTestData))]
        public async Task Get_performance_attributes_from_difficulty(int beatmapId, BasicScoreInfo info, double pp) {
            var difficultyAttributes = await RequestUtilities.RequestDifficulty(client, beatmapId, info.Mods);
            // avoid redundant mods in query string from score info
            difficultyAttributes!.Mods = Array.Empty<Mod>();
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(difficultyAttributes, SerializeScoreProperty);
            query.AddObject(info, SerializeScoreProperty);
            builder.Query = query.ToString();

            await Get_performance_attributes(builder, pp);
        }

        [Theory]
        [MemberData(nameof(RawBeatmapTestData))]
        public async Task Get_performance_attributes_from_uploaded_file(byte[] data, BasicScoreInfo info, double pp) {
            var builder = new UriBuilder(new Uri(client.BaseAddress!, $"api/performance"));
            var query = HttpUtility.ParseQueryString(builder.Query);
            query.AddObject(info, SerializeScoreProperty);
            builder.Query = query.ToString();

            var response = await client.PostAsync(builder.Uri, data.ToFormContent());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            var attributes = JsonSerializer.Deserialize<OsuPerformanceAttributes>(body, RequestUtilities.SerializerOptions);

            Assert.NotNull(attributes);
            Assert.InRange(attributes!.Total, pp - 0.05, pp + 0.05);
        }
    }
}