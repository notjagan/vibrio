using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using vibrio.src;

namespace vibrio.Beatmaps {
    public class LocalBeatmapCache : IBeatmapProvider {
        private readonly string _cacheDirectory;
        private readonly string _osuRootUrl;

        public LocalBeatmapCache(IConfiguration config) {
            _cacheDirectory = config["CacheDirectory"];
            if (_cacheDirectory == null) {
                throw new MissingConfigurationException("No configuration value for beatmap cache directory provided.");
            }
            _osuRootUrl = config["OsuRootUrl"];
            if (_osuRootUrl == null) {
                throw new MissingConfigurationException("No configuration value for osu! URL provided.");
            }
        }

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            var beatmapPath = Path.Combine(_cacheDirectory, beatmapId.ToString());
            if (!File.Exists(beatmapPath)) {
                new FileWebRequest(beatmapPath, $"{_osuRootUrl}/osu/{beatmapId}").Perform();
            }

            return new FlatFileWorkingBeatmap(beatmapPath);
        }
    }
}
