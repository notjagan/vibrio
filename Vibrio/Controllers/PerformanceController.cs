using Microsoft.AspNetCore.Mvc;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Scoring;
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

            var attributes = DifficultyController.GetDifficulty(beatmap, info.Mods);
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpGet]
        public ActionResult<OsuPerformanceAttributes> GetPerformance([FromQuery] OsuDifficultyAttributes attributes, [FromQuery] BasicScoreInfo info) {
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpPost]
        public async Task<ActionResult<OsuPerformanceAttributes>> GetPerformance(IFormFile beatmap, [FromQuery] BasicScoreInfo info) {
            var b = await beatmap.LoadBeatmap();
            if (b.Beatmap.HitObjects.Count == 0) {
                return BadRequest("Invalid/empty beatmap file");
            }

            var attributes = DifficultyController.GetDifficulty(b, info.Mods);
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info.GetScoreInfo(), attributes);
        }

        [HttpPost("replay/{beatmapId}")]
        public async Task<ActionResult<OsuPerformanceAttributes>> GetPerformance(IFormFile replay, int beatmapId) {
            WorkingBeatmap beatmap;
            try {
                beatmap = beatmaps.GetBeatmap(beatmapId);
            } catch (BeatmapNotFoundException) {
                return NotFound($"Beatmap with id {beatmapId} not found");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return StatusCode(500);
            }

            var decoder = new BeatmapLegacyScoreDecoder(beatmap);
            using var stream = new MemoryStream();
            await replay.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            ScoreInfo info;
            try {
                info = decoder.Parse(stream).ScoreInfo;
            } catch (BeatmapMismatchException ex) {
                return BadRequest(ex.Message);
            }

            var attributes = DifficultyController.GetDifficulty(beatmap, info.Mods);
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info, attributes);
        }

        [HttpPost("replay")]
        public async Task<ActionResult<OsuPerformanceAttributes>> GetPerformance(IFormFile replay, IFormFile beatmap) {
            var b = await beatmap.LoadBeatmap();
            if (b.Beatmap.HitObjects.Count == 0) {
                return BadRequest("Invalid/empty beatmap file");
            }

            var decoder = new BeatmapLegacyScoreDecoder(b);
            using var stream = new MemoryStream();
            await replay.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            ScoreInfo info;
            try {
                info = decoder.Parse(stream).ScoreInfo;
            } catch (BeatmapMismatchException ex) {
                return BadRequest(ex.Message);
            }

            var attributes = DifficultyController.GetDifficulty(b, info.Mods);
            return (OsuPerformanceAttributes)new OsuPerformanceCalculator().Calculate(info, attributes);
        }
    }
}
