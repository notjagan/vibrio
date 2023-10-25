using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace vibrio.src.Models {
    public class BasicScoreInfo {
        public int Count300 { get; set; }
        public int Count100 { get; set; }
        public int Count50 { get; set; }
        public int CountMiss { get; set; }
        public int Combo { get; set; }
        public Mod[] Mods { get; set; } = Array.Empty<Mod>();

        public double GetAccuracy() {
            var total = Count300 + Count100 + Count50 + CountMiss;
            return (double)((6 * Count300) + (2 * Count100) + Count50) / (6 * total);
        }

        public ScoreInfo GetScoreInfo() => new() {
            Accuracy = GetAccuracy(),
            MaxCombo = Combo,
            Statistics = new Dictionary<HitResult, int> {
                { HitResult.Great, Count300 },
                { HitResult.Ok, Count100 },
                { HitResult.Meh, Count50 },
                { HitResult.Miss, CountMiss }
            },
            Mods = Mods
        };
    }
}
