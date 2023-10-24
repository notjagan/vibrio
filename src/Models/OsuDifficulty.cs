using osu.Game.Rulesets.Osu.Difficulty;
using System.Text.Json.Serialization;
using vibrio.src.Utilities;

namespace vibrio.Models {
    public class OsuDifficulty {
        public double StarRating { get; set; }
        public int MaxCombo { get; set; }
        public double AimDifficulty { get; set; }
        public double SpeedDifficulty { get; set; }
        public double SpeedNoteCount { get; set; }
        public double FlashlightDifficulty { get; set; }
        public double SliderFactor { get; set; }
        public double ApproachRate { get; set; }
        public double DrainRate { get; set; }
        public int HitCircleCount { get; set; }
        public int SliderCount { get; set; }
        public int SpinnerCount { get; set; }
        [JsonConverter(typeof(ModContainerListConverter))]
        public ModContainer[] Mods { get; set; }

        public OsuDifficulty(OsuDifficultyAttributes attributes) {
            StarRating = attributes.StarRating;
            MaxCombo = attributes.MaxCombo;
            AimDifficulty = attributes.AimDifficulty;
            SpeedDifficulty = attributes.SpeedDifficulty;
            SpeedNoteCount = attributes.SpeedNoteCount;
            FlashlightDifficulty = attributes.FlashlightDifficulty;
            SliderFactor = attributes.SliderFactor;
            ApproachRate = attributes.ApproachRate;
            DrainRate = attributes.DrainRate;
            HitCircleCount = attributes.HitCircleCount;
            SliderCount = attributes.SliderCount;
            SpinnerCount = attributes.SpinnerCount;
            Mods = attributes.Mods.Select(mod => new ModContainer(mod)).ToArray();
        }
    }
}
