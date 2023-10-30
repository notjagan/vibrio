using Microsoft.AspNetCore.Mvc;
using osu.Game.Beatmaps;
using System.Net.Mime;
using Vibrio.Exceptions;
using Vibrio.Models;

namespace Vibrio.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BeatmapsController : ControllerBase {
        private readonly IBeatmapProvider beatmaps;

        public BeatmapsController(IBeatmapProvider beatmaps) {
            this.beatmaps = beatmaps;
        }

        [HttpDelete("cache")]
        public void ClearCache() {
            try {
                beatmaps.ClearCache();
            } catch (NotImplementedException) { }
        }

        [HttpGet("{beatmapId}/status")]
        public ActionResult HasBeatmap(int beatmapId) {
            if (beatmaps.HasBeatmap(beatmapId)) {
                return Ok();
            } else {
                return NotFound("Beatmap not stored");
            }
        }

        [HttpGet("{beatmapId}")]
        public ActionResult GetBeatmap(int beatmapId) {
            WorkingBeatmap beatmap;
            try {
                beatmap = beatmaps.GetBeatmap(beatmapId);
            } catch (BeatmapNotFoundException) {
                return NotFound($"Beatmap with id {beatmapId} not found");
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return StatusCode(500);
            }

            return File(beatmaps.GetBeatmapStream(beatmapId), MediaTypeNames.Application.Octet, $"{beatmapId}.osu");
        }
    }
}
