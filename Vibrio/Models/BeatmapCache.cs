using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using Vibrio.Exceptions;
using Vibrio.Tests.Utilities;

namespace Vibrio.Models {
    public abstract class BeatmapCache : IBeatmapProvider {
        private readonly string osuRootUrl;

        public BeatmapCache(AppConfiguration config) {
            if (!config.UseCaching) {
                throw new MissingConfigurationException("Caching is not enabled.");
            }
            osuRootUrl = config.OsuRootUrl;
        }

        public abstract string CacheDirectory();

        public string BeatmapPath(int beatmapId) => Path.Combine(CacheDirectory(), Path.ChangeExtension(beatmapId.ToString(), "osu"));

        public void ClearCache() {
            var info = new DirectoryInfo(CacheDirectory());
            if (info.Exists) {
                info.Delete(true);
            }
        }

        public bool HasBeatmap(int beatmapId) => File.Exists(BeatmapPath(beatmapId));

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            var path = BeatmapPath(beatmapId);
            if (!File.Exists(path)) {
                new FileWebRequest(path, $"{osuRootUrl}/osu/{beatmapId}").Perform();
            }

            try {
                using var file = File.OpenRead(path);
                return file.LoadBeatmap();
            } catch (IOException) {
                throw new BeatmapNotFoundException("Invalid online beatmap");
            }
        }

        public Stream GetBeatmapStream(int beatmapId) {
            var path = BeatmapPath(beatmapId);
            if (!File.Exists(path)) {
                new FileWebRequest(path, $"{osuRootUrl}/osu/{beatmapId}").Perform();
            }

            if (new FileInfo(path).Length == 0) {
                throw new BeatmapNotFoundException("Invalid/empty online beatmap");
            }

            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
