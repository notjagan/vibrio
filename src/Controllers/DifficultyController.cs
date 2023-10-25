﻿using Microsoft.AspNetCore.Mvc;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Objects;
using vibrio.Beatmaps;

namespace vibrio.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase {
        private static Ruleset ruleset = new OsuRuleset();

        private readonly IBeatmapProvider beatmaps;

        public DifficultyController(IBeatmapProvider beatmaps) {
            this.beatmaps = beatmaps;
            ruleset = new OsuRuleset();
        }

        public static OsuDifficultyAttributes GetDifficulty(WorkingBeatmap beatmap, IEnumerable<Mod> mods) {
            var calculator = new OsuDifficultyCalculator(ruleset.RulesetInfo, beatmap);
            var attributes = (OsuDifficultyAttributes)calculator.Calculate(mods);

            // max combo reported by performance calculator doesn't match stable combo for some reason
            var playableMap = beatmap.GetPlayableBeatmap(ruleset.RulesetInfo);
            int maxCombo = playableMap.HitObjects.Count + playableMap.HitObjects.OfType<Slider>().Sum(slider => slider.NestedHitObjects.Count - 1);
            attributes.MaxCombo = maxCombo;

            return attributes;
        }

        [HttpGet("{beatmapId}")]
        public ActionResult<OsuDifficultyAttributes> GetDifficulty(int beatmapId, [FromQuery] Mod[] mods) {
            if (mods.Any(mod => mod == null)) {
                return BadRequest("Unrecognized mod");
            }
            var beatmap = beatmaps.GetBeatmap(beatmapId);
            return GetDifficulty(beatmap, mods);
        }
    }
}
