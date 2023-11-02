using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using Vibrio.Models;

namespace Vibrio.Tests.Controllers {
    public static class PerformanceControllerTestData {
        public record TestBeatmap {
            public int Id;
            public byte[] BeatmapData;
            public byte[] ReplayData;
            public Mod[] Mods;
            public BasicScoreInfo Info;
            public double Pp;

            public TestBeatmap(int id, byte[] data, byte[] replayData, Mod[] mods, BasicScoreInfo info, double pp) {
                Id = id;
                BeatmapData = data;
                ReplayData = replayData;
                Mods = mods;
                Info = info;
                Pp = pp;
            }
        }

        public static readonly TestBeatmap[] TestData = new[] {
            new TestBeatmap(
                1001682,
                Properties.Resources._1001682_osu,
                Properties.Resources._4429758207_osr,
                new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                new BasicScoreInfo {
                    Mods = new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                    Count300 = 2019,
                    Count100 = 104,
                    Count50 = 0,
                    CountMiss = 3,
                    Combo = 3141
                },
                1304.35
            ),
            new TestBeatmap(
                2042429,
                Properties.Resources._2042429_osu,
                Properties.Resources._3974524967_osr,
                new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                new BasicScoreInfo {
                    Mods = new Mod[] { new OsuModHidden(), new OsuModDoubleTime() },
                    Count300 = 1008,
                    Count100 = 53,
                    Count50 = 0,
                    CountMiss = 6,
                    Combo = 1183
                },
                931.837
            ),
        };
    }
}
