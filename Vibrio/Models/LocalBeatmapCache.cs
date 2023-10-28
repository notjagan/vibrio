﻿using osu.Framework.IO.Network;
using osu.Game.Beatmaps;
using Vibrio.Exceptions;

namespace Vibrio.Models {
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

            return new FlatFileWorkingBeatmap(path);
        }
    }
}