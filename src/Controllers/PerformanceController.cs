using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Rules;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Scoring;
using System.Net;
using vibrio.Beatmaps;
using vibrio.Controllers;
using vibrio.src.Models;

namespace vibrio.src.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase {
        private readonly IBeatmapProvider beatmaps;

        public PerformanceController(IBeatmapProvider beatmaps) {
            this.beatmaps = beatmaps;
        }

        [HttpGet("{beatmapId}")]
        public ActionResult<OsuPerformanceAttributes> GetPerformance(int beatmapId, [FromQuery] BasicScoreInfo info) {
            WorkingBeatmap beatmap;
            try {
                beatmap = beatmaps.GetBeatmap(beatmapId);
            } catch (IOException) {
                return NotFound($"Beatmap with id {beatmapId} not found");
            } catch (WebException exception) {
                Console.WriteLine(exception.ToString());
                return StatusCode(500);
            }

            var attributes = DifficultyController.GetDifficulty(beatmap, info.Mods.AsEnumerable());
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpGet]
        public ActionResult<OsuPerformanceAttributes> GetPerformance([FromQuery] OsuDifficultyAttributes attributes, [FromQuery] BasicScoreInfo info) {
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }
    }
}
