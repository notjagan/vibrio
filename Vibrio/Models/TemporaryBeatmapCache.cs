namespace Vibrio.Models {
    public class TemporaryBeatmapCache : BeatmapCache {
        private readonly string directory;

        public TemporaryBeatmapCache(AppConfiguration config)
            : base(config) {
            directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }

        public override string CacheDirectory() => directory;
    }
}
