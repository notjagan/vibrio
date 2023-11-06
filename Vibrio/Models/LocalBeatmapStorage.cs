using osu.Game.Beatmaps;
using osu.Game.IO;
using Vibrio.Exceptions;
using Vibrio.Tests.Utilities;

namespace Vibrio.Models {
    public class LocalBeatmapStorage : IBeatmapProvider {
        private readonly Dictionary<int, string> registry;
        private readonly string songsFolder;

        public LocalBeatmapStorage(AppConfiguration config) {
            var osuFolder = config.OsuInstallFolder
                ?? throw new MissingConfigurationException("No configuration value for osu! folder provided.");
            using var file = File.OpenRead(Path.Combine(osuFolder, "osu!.db"));
            registry = OsuDb.OsuDb.LoadBeatmapRegistry(file);

            var storage = new StableStorage(osuFolder, null);
            songsFolder = storage.GetSongStorage().GetFullPath(".");
        }

        private string GetBeatmapPath(int beatmapId) => Path.Combine(songsFolder, registry[beatmapId]);

        public void ClearCache() => throw new NotImplementedException();

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            if (!registry.ContainsKey(beatmapId)) {
                throw new BeatmapNotFoundException("Beatmap not in osu! songs folder");
            }

            try {
                using var file = File.OpenRead(GetBeatmapPath(beatmapId));
                return file.LoadBeatmap();
            } catch (Exception ex) when (ex is IOException || ex is FileNotFoundException) {
                throw new BeatmapNotFoundException("Missing beatmap");
            }
        }

        public Stream GetBeatmapStream(int beatmapId) {
            if (!registry.ContainsKey(beatmapId)) {
                throw new BeatmapNotFoundException("Beatmap not in osu! songs folder");
            }

            try {
                return File.OpenRead(GetBeatmapPath(beatmapId));
            } catch (Exception ex) when (ex is IOException || ex is FileNotFoundException) {
                throw new BeatmapNotFoundException("Missing beatmap");
            }
        }

        public bool HasBeatmap(int beatmapId) => registry.ContainsKey(beatmapId);
    }
}
