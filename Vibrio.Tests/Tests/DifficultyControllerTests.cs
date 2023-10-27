using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using vibrio.Controllers;
using Vibrio.Tests.Utilities;

namespace Vibrio.Tests.Tests {
    public class DifficultyControllerTests {
        public static IEnumerable<object[]> TestData =>
            new List<object[]> {
                new object[] { Properties.Resources._1001682_osu, Array.Empty<Mod>(), 6.38, 3220 },
                new object[] { Properties.Resources._1001682_osu, new Mod[] { new OsuModDoubleTime() }, 9.7, 3220 },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void Verify_difficulty_attributes(byte[] beatmapData, Mod[] mods, float starRating, int maxCombo) {
            var beatmap = beatmapData.LoadBeatmap();
            var attributes = DifficultyController.GetDifficulty(beatmap, mods);
            Assert.InRange(attributes.StarRating, starRating - 0.05, starRating + 0.05);
            Assert.Equal(maxCombo, attributes.MaxCombo);
        }
    }
}