using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using vibrio.src.Exceptions;

namespace vibrio.Beatmaps {
    public class LocalBeatmapCache : IBeatmapProvider {
        private readonly string cacheDirectory;
        private readonly string osuRootUrl;

        public LocalBeatmapCache(IConfiguration config) {
            cacheDirectory = config["CacheDirectory"];
            if (cacheDirectory == null) {
                throw new MissingConfigurationException("No configuration value for beatmap cache directory provided.");
            }
            osuRootUrl = config["OsuRootUrl"];
            if (osuRootUrl == null) {
                throw new MissingConfigurationException("No configuration value for osu! URL provided.");
            }
        }

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            var beatmapPath = Path.Combine(cacheDirectory, beatmapId.ToString());
            if (!File.Exists(beatmapPath)) {
                new FileWebRequest(beatmapPath, $"{osuRootUrl}/osu/{beatmapId}").Perform();
            }

            return new FlatFileWorkingBeatmap(beatmapPath);
        }
    }
}
