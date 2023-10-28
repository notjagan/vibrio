using Microsoft.Extensions.Configuration;
using Vibrio.Models;

namespace Vibrio.Tests.Tests {
    public class LocalBeatmapCacheTests : IDisposable {
        private readonly LocalBeatmapCache cache;

        public LocalBeatmapCacheTests() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();
            cache = new LocalBeatmapCache(config);
            cache.ClearCache();
        }

        public void Dispose() {
            cache.ClearCache();
            GC.SuppressFinalize(this);
        }

        [Theory]
        [InlineData(1001682)]
        [InlineData(2042429)]
        public void Lookup_beatmap(int beatmapId) {
            Assert.False(cache.HasBeatmap(beatmapId));
            var beatmap = cache.GetBeatmap(beatmapId);
            Assert.NotNull(beatmap);
            Assert.Equal(beatmap.BeatmapInfo.OnlineID, beatmapId);
        }
    }
}
