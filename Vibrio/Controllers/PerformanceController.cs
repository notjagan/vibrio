using Microsoft.AspNetCore.Mvc;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu.Difficulty;
using Vibrio.Exceptions;
using Vibrio.Models;
using Vibrio.Tests.Utilities;

namespace Vibrio.Controllers {
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
            } catch (BeatmapNotFoundException) {
                return NotFound($"Beatmap with id {beatmapId} not found");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return StatusCode(500);
            }

            var attributes = DifficultyController.GetDifficulty(beatmap, info.Mods.AsEnumerable());
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpGet]
        public ActionResult<OsuPerformanceAttributes> GetPerformance([FromQuery] OsuDifficultyAttributes attributes, [FromQuery] BasicScoreInfo info) {
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpPost]
        public async Task<ActionResult<OsuPerformanceAttributes>> GetPerformance(IFormFile file, [FromQuery] BasicScoreInfo info) {
            try {
                var beatmap = await file.LoadBeatmap();
                if (beatmap.Beatmap.HitObjects.Count == 0) {
                    return BadRequest("Invalid/empty beatmap file");
                }

                var attributes = DifficultyController.GetDifficulty(beatmap, info.Mods.AsEnumerable());
                return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
            } catch (Exception) {
                return BadRequest($"Error while processing file");
            }
        }
    }
}
