using osu.Game.Beatmaps;

namespace Vibrio.Models {
    public interface IBeatmapProvider {
        WorkingBeatmap GetBeatmap(int beatmapId);

        Stream GetBeatmapStream(int beatmapId);

        void ClearCache();

        bool HasBeatmap(int beatmapId);
    }
}
