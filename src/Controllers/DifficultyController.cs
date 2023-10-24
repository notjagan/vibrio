using Microsoft.AspNetCore.Mvc;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Mods;
using vibrio.Beatmaps;
using vibrio.Models;

namespace vibrio.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase {
        private readonly IBeatmapProvider _beatmaps;

        public DifficultyController(IBeatmapProvider beatmapsOptions) {
            _beatmaps = beatmapsOptions;
        }

        [HttpGet]
        public OsuDifficulty GetDifficulty(int beatmapId) {
            var beatmap = _beatmaps.GetBeatmap(beatmapId);
            var calculator = new OsuDifficultyCalculator(new OsuRuleset().RulesetInfo, beatmap);
            return new OsuDifficulty((OsuDifficultyAttributes)calculator.Calculate(new Mod[] { new OsuModClassic() }));
        }
    }
}
