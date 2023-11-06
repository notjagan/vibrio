using Vibrio.Exceptions;

namespace Vibrio.Models {
    public class PersistentBeatmapCache : BeatmapCache {
        private readonly string directory;

        public PersistentBeatmapCache(AppConfiguration config)
            : base(config) {
            directory = config.CacheDirectory
                ?? throw new MissingConfigurationException("No configuration value for beatmap cache directory provided.");
        }

        public override string CacheDirectory() => directory;
    }
}
