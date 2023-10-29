using Microsoft.AspNetCore.Mvc;
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
            beatmaps.ClearCache();
        }

        [HttpGet("{beatmapId}/status")]
        public ActionResult HasBeatmap(int beatmapId) {
            if (beatmaps.HasBeatmap(beatmapId)) {
                return Ok();
            } else {
                return NotFound("Beatmap not stored");
            }
        }
    }
}
