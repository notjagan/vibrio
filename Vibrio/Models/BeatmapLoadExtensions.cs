using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using System.Security.Cryptography;

namespace Vibrio.Tests.Utilities {
    public static class BeatmapLoadExtensions {
        public static string MD5Hash(Stream stream) {
            stream.Seek(0, SeekOrigin.Begin);
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static WorkingBeatmap LoadBeatmap(this Stream stream) {
            using var reader = new LineBufferedReader(stream);
            // constructor for FlatFileWorkingBeatmap accepting an IBeatmap is currently private, can be simplified in a future version
            var beatmap = new SimpleWorkingBeatmap(Decoder.GetDecoder<Beatmap>(reader).Decode(reader));
            beatmap.BeatmapInfo.Hash = MD5Hash(stream);
            return beatmap;
        }

        public async static Task<WorkingBeatmap> LoadBeatmap(this IFormFile file) {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream.LoadBeatmap();
        }

        public async static Task<WorkingBeatmap> LoadBeatmap(this HttpContent content) {
            using var stream = new MemoryStream();
            await content.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream.LoadBeatmap();
        }

        public static WorkingBeatmap LoadBeatmap(this byte[] content) {
            using var stream = new MemoryStream(content);
            return stream.LoadBeatmap();
        }
    }
}
