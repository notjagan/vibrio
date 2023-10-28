using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vibrio.Models {
    public class ModConverter : JsonConverter<Mod> {
        public override bool CanConvert(Type typeToConvert) {
            return typeToConvert == typeof(Mod) || typeToConvert.IsSubclassOf(typeof(Mod)) || base.CanConvert(typeToConvert);
        }

        public override Mod? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var converter = (JsonConverter<string>)options.GetConverter(typeof(string));
            return new OsuRuleset().CreateModFromAcronym(converter.Read(ref reader, typeof(string), options)!);
        }

        public override void Write(Utf8JsonWriter writer, Mod value, JsonSerializerOptions options) {
            var converter = (JsonConverter<string>)options.GetConverter(typeof(string));
            converter.Write(writer, value.Acronym, options);
        }
    }
}
