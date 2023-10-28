using Microsoft.AspNetCore.Mvc;
using Vibrio.Models;

namespace Vibrio.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CacheController : ControllerBase {
        private readonly IBeatmapProvider beatmaps;

        public CacheController(IBeatmapProvider beatmaps) {
            this.beatmaps = beatmaps;
        }

        [HttpDelete]
        public void ClearCache() {
            beatmaps.ClearCache();
        }

        [HttpGet("{beatmapId}/status")]
        public ActionResult HasBeatmap(int beatmapId) {
            if (beatmaps.HasBeatmap(beatmapId)) {
                return Ok();
            } else {
                return NotFound("Beatmap not in cache");
            }
        }
    }
}
