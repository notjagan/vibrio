namespace Vibrio.Models {
    public class TemporaryBeatmapCache : BeatmapCache {
        private readonly string directory;

        public TemporaryBeatmapCache(IConfiguration config)
            : base(config) {
            directory = Guid.NewGuid().ToString();
        }

        public override string CacheDirectory() => directory;
    }
}
