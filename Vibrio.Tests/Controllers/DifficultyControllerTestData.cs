using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;

namespace Vibrio.Tests.Controllers {
    public static class DifficultyControllerTestData {
        public record TestBeatmap {
            public int Id;
            public byte[] Data;
            public Mod[] Mods;
            public double StarRating;
            public int MaxCombo;

            public TestBeatmap(int id, byte[] data, Mod[] mods, double starRating, int maxCombo) {
                Id = id;
                Data = data;
                Mods = mods;
                StarRating = starRating;
                MaxCombo = maxCombo;
            }
        }

        public static readonly TestBeatmap[] TestData = new[] {
            new TestBeatmap(1001682, Properties.Resources._1001682_osu, Array.Empty<Mod>(), 6.38, 3220),
            new TestBeatmap(1001682, Properties.Resources._1001682_osu, new Mod[] { new OsuModDoubleTime() }, 9.7, 3220),
            new TestBeatmap(2042429, Properties.Resources._2042429_osu, Array.Empty<Mod>(), 7.4, 1460),
            new TestBeatmap(2042429, Properties.Resources._2042429_osu, new Mod[] { new OsuModHidden(), new OsuModFlashlight() }, 9.21, 1460),
        };
    }
}
