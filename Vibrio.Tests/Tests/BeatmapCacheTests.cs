using Microsoft.Extensions.Configuration;
using Vibrio.Models;

namespace Vibrio.Tests.Tests {
    public class BeatmapCacheTests : IDisposable {
        private readonly PersistentBeatmapCache cache;

        public BeatmapCacheTests() {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();
            cache = new PersistentBeatmapCache(config);
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
            Assert.True(cache.HasBeatmap(beatmapId));
        }
    }
}
