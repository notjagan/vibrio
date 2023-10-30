using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;

namespace Vibrio.Tests.Utilities {
    public static class BeatmapLoadExtension {
        public async static Task<WorkingBeatmap> LoadBeatmap(this IFormFile file) {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new LineBufferedReader(stream);
            return new SimpleWorkingBeatmap(Decoder.GetDecoder<Beatmap>(reader).Decode(reader));
        }

        public async static Task<WorkingBeatmap> LoadBeatmap(this HttpContent content) {
            using var stream = new MemoryStream();
            await content.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new LineBufferedReader(stream);
            return new SimpleWorkingBeatmap(Decoder.GetDecoder<Beatmap>(reader).Decode(reader));
        }

        public static WorkingBeatmap LoadBeatmap(this byte[] content) {
            var stream = new MemoryStream(content);
            using var reader = new LineBufferedReader(stream);
            // constructor for FlatFileWorkingBeatmap accepting an IBeatmap is currently private, can be simplified in a future version
            var beatmap = new SimpleWorkingBeatmap(Decoder.GetDecoder<Beatmap>(reader).Decode(reader));
            return beatmap;
        }
    }
}
