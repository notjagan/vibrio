﻿using Microsoft.AspNetCore.Mvc;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using vibrio.Beatmaps;

namespace vibrio.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase {
        private static Ruleset ruleset = new OsuRuleset();

        private readonly IBeatmapProvider beatmaps;

        public DifficultyController(IBeatmapProvider beatmapsOptions) {
            beatmaps = beatmapsOptions;
            ruleset = new OsuRuleset();
        }

        public static OsuDifficultyAttributes GetDifficulty(WorkingBeatmap beatmap, IEnumerable<Mod> mods) {
            var calculator = new OsuDifficultyCalculator(ruleset.RulesetInfo, beatmap);
            return (OsuDifficultyAttributes)calculator.Calculate(mods);
        }

        [HttpGet]
        public ActionResult<OsuDifficultyAttributes> GetDifficulty(int beatmapId, [FromQuery] Mod[] mods) {
            if (mods.Any(mod => mod == null)) {
                return BadRequest("Unrecognized mod");
            }
            var beatmap = beatmaps.GetBeatmap(beatmapId);
            return GetDifficulty(beatmap, mods);
        }
    }
}
