﻿using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using Vibrio.Exceptions;

namespace Vibrio.Models {
    public class BeatmapCache : IBeatmapProvider {
        private readonly string cacheDirectory;
        private readonly string osuRootUrl;

        public BeatmapCache(IConfiguration config) {
            cacheDirectory = config["CacheDirectory"]
                ?? throw new MissingConfigurationException("No configuration value for beatmap cache directory provided.");
            osuRootUrl = config["OsuRootUrl"]
                ?? throw new MissingConfigurationException("No configuration value for osu! URL provided.");
        }

        public void ClearCache() {
            var info = new DirectoryInfo(cacheDirectory);
            if (info.Exists) {
                info.Delete(true);
            }
        }

        private string BeatmapPath(int beatmapId) => Path.Combine(cacheDirectory, $"{beatmapId}.osu");

        public bool HasBeatmap(int beatmapId) => File.Exists(BeatmapPath(beatmapId));

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            var path = BeatmapPath(beatmapId);
            if (!File.Exists(path)) {
                new FileWebRequest(path, $"{osuRootUrl}/osu/{beatmapId}").Perform();
            }

            try {
                return new FlatFileWorkingBeatmap(path);
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
