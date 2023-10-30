using osu.Game.Beatmaps;
using Vibrio.Exceptions;
using Vibrio.Tests.Utilities;

namespace Vibrio.Models {
    public class BeatmapDirectDownload : IBeatmapProvider {
        private readonly HttpClient client;

        public BeatmapDirectDownload(IConfiguration config) {
            var osuRootUrl = config["OsuRootUrl"]
                ?? throw new MissingConfigurationException("No configuration value for osu! URL provided.");
            client = new HttpClient { BaseAddress = new Uri(osuRootUrl) };
        }

        public void ClearCache() => throw new NotImplementedException();

        public WorkingBeatmap GetBeatmap(int beatmapId) {
            HttpResponseMessage response;
            try {
                response = client.GetAsync($"osu/{beatmapId}").Result;
                return response.Content.LoadBeatmap().Result;
            } catch (Exception) {
                throw new BeatmapNotFoundException("Could not download beatmap");
            }
        }

        public Stream GetBeatmapStream(int beatmapId) {
            HttpResponseMessage response;
            try {
                response = client.GetAsync($"osu/{beatmapId}").Result;
            } catch (Exception) {
                throw new BeatmapNotFoundException("Could not download beatmap");
            }

            if (response.Content.Headers.ContentLength == 0) {
                throw new BeatmapNotFoundException("Invalid/empty online beatmap");
            }

            var stream = new MemoryStream();
            response.Content.CopyToAsync(stream).Wait();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public bool HasBeatmap(int beatmapId) => false;
    }
}
