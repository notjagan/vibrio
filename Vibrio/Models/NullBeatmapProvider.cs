using osu.Game.Beatmaps;

namespace Vibrio.Models {
    public class NullBeatmapProvider : IBeatmapProvider {
        public void ClearCache() => throw new NotImplementedException();
        public WorkingBeatmap GetBeatmap(int beatmapId) => throw new NotImplementedException();
        public Stream GetBeatmapStream(int beatmapId) => throw new NotImplementedException();
        public bool HasBeatmap(int beatmapId) => throw new NotImplementedException();
    }
}
