using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;

namespace Vibrio.Tests.Utilities {
    public static class BeatmapLoadExtension {
        public static WorkingBeatmap LoadBeatmap(this byte[] file) {
            var stream = new MemoryStream(file);
            using var reader = new LineBufferedReader(stream);
            // constructor for FlatFileWorkingBeatmap accepting an IBeatmap is currently private, can be simplified in a future version
            var beatmap = new SimpleWorkingBeatmap(Decoder.GetDecoder<Beatmap>(reader).Decode(reader));
            return beatmap;
        }
    }
}
