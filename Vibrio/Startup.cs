using Microsoft.OpenApi.Models;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using Vibrio.Exceptions;
using Vibrio.Models;

namespace Vibrio {
    public class Startup {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration.Get<AppConfiguration>();
            IBeatmapProvider beatmaps = new BeatmapDirectDownload(config);

            try {
                beatmaps = new TemporaryBeatmapCache(config) | beatmaps;
            } catch (MissingConfigurationException) { }
            try {
                beatmaps = new PersistentBeatmapCache(config) | beatmaps;
            } catch (MissingConfigurationException) { }
            try {
                beatmaps = new LocalBeatmapStorage(config) | beatmaps;
            } catch (MissingConfigurationException) { }

            builder.Services.AddSingleton(beatmaps);

            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            var ruleset = new OsuRuleset();
            builder.Services.AddControllers()
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.Converters.Add(new ModConverter());
                })
                .AddMvcOptions(options => {
                    options.ModelBinderProviders.Insert(0, new ModListModelBinderProvider(ruleset));
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                options.MapType<Mod>(() => new OpenApiSchema { Type = "string", Format = null });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("api/status", () => Results.Ok());

            app.Run();
        }
    }
}
