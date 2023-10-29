﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using osu.Game.Rulesets.Mods;
using System.Net;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Tests {
    public class BeatmapsControllerTests
        : IClassFixture<WebApplicationFactory<Startup>>, IDisposable {
        private readonly HttpClient client;

        public BeatmapsControllerTests(WebApplicationFactory<Startup> factory) {
            client = factory.WithWebHostBuilder(builder => builder.UseEnvironment("Development")).CreateClient();
            client.DeleteAsync("api/beatmaps/cache").Wait();
        }

        public async void Dispose() {
            await client.DeleteAsync("api/beatmaps/cache");
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        [Theory]
        [InlineData(1001682)]
        [InlineData(2042429)]
        public async Task Check_local_cache_status_after_difficulty_request(int beatmapId) {
            var endpoint = $"api/beatmaps/{beatmapId}/status";
            var response = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await RequestUtilities.RequestDifficulty(client, beatmapId, Array.Empty<Mod>());

            response = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.DeleteAsync("api/beatmaps/cache");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            response = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
