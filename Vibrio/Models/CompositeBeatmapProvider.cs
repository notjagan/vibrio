﻿using osu.Game.Beatmaps;
using Vibrio.Exceptions;

namespace Vibrio.Models {
    public class CompositeBeatmapProvider : IBeatmapProvider {
        private readonly IBeatmapProvider first;
        private readonly IBeatmapProvider? second;

        public CompositeBeatmapProvider(IBeatmapProvider first, IBeatmapProvider? second) {
            this.first = first;
            this.second = second;
        }

        public void ClearCache() {
            try {
                first.ClearCache();
            } catch (NotImplementedException) { }
            try {
                second?.ClearCache();
            } catch (NotImplementedException) { }
        }

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            WorkingBeatmap beatmap;
            try {
                beatmap = first.GetBeatmap(beatmapId);
            } catch (BeatmapNotFoundException exception) {
                if (second == null) {
                    throw exception;
                }
                beatmap = second.GetBeatmap(beatmapId);
            }

            return beatmap;
        }

        public Stream GetBeatmapStream(int beatmapId) {
            Stream stream;
            try {
                stream = first.GetBeatmapStream(beatmapId);
            } catch (BeatmapNotFoundException exception) {
                if (second == null) {
                    throw exception;
                }
                stream = second.GetBeatmapStream(beatmapId);
            }

            return stream;
        }

        public bool HasBeatmap(int beatmapId) => first.HasBeatmap(beatmapId) || (second?.HasBeatmap(beatmapId) ?? false);
    }
}
