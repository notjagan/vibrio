using osu.Game.Beatmaps;

namespace vibrio.Beatmaps {
    public interface IBeatmapProvider {
        WorkingBeatmap GetBeatmap(int beatmapId);
    }
}
