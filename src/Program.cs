using Microsoft.OpenApi.Models;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using vibrio.Beatmaps;
using vibrio.src.Models;

namespace vibrio.src {
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            builder.Services.AddSingleton<IBeatmapProvider>(new LocalBeatmapCache(config));

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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}