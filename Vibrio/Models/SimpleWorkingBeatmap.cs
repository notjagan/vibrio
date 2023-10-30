using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Skinning;

namespace Vibrio.Tests.Utilities {
    public class SimpleWorkingBeatmap : WorkingBeatmap {
        private readonly IBeatmap beatmap;

        public SimpleWorkingBeatmap(IBeatmap beatmap)
            : base(beatmap.BeatmapInfo, null) {
            this.beatmap = beatmap;
        }

        protected override IBeatmap GetBeatmap() => beatmap;
        public override Stream GetStream(string storagePath) => throw new NotImplementedException();
        protected override Texture GetBackground() => throw new NotImplementedException();
        protected override Track GetBeatmapTrack() => throw new NotImplementedException();
        protected override ISkin GetSkin() => throw new NotImplementedException();
    }
}
