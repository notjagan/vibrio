using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using Vibrio.Models;

namespace Vibrio.Tests.Tests {
    public static class PerformanceControllerTestData {
        public record TestBeatmap {
            public int Id;
            public byte[] Data;
            public Mod[] Mods;
            public BasicScoreInfo Info;
            public double Pp;

            public TestBeatmap(int id, byte[] data, Mod[] mods, BasicScoreInfo info, double pp) {
                Id = id;
                Data = data;
                Mods = mods;
                Info = info;
                Pp = pp;
            }
        }

        public static readonly TestBeatmap[] TestData = new[] {
            new TestBeatmap(
                1001682,
                Properties.Resources._1001682_osu,
                new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                new BasicScoreInfo{
                    Mods = new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                    Count300 = 2065,
                    Count100 = 59,
                    Count50 = 0,
                    CountMiss = 2,
                    Combo = 2572,
                },
                1233.04
            ),
        };
    }
}
