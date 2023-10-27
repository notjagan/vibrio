using osu.Game.Beatmaps;

namespace vibrio.src.Models {
    public interface IBeatmapProvider {
        WorkingBeatmap GetBeatmap(int beatmapId);
        void ClearCache();
        bool HasBeatmap(int beatmapId);
    }
}
