namespace Vibrio.Models {
    public class AppConfiguration {
        public string OsuRootUrl { get; init; } = "https://osu.ppy.sh";
        public bool UseCaching { get; init; } = true;
        public string? CacheDirectory { get; init; }
        public string? OsuInstallFolder { get; init; }
    }
}
