using System.Text.Json;
using System.Text.Json.Serialization;

namespace vibrio.src.Utilities {
    public class ModContainerListConverter : JsonConverter<ModContainer[]> {
        public override ModContainer[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ModContainer[] value, JsonSerializerOptions options) {
            var converter = (JsonConverter<IEnumerable<string>>)options.GetConverter(typeof(IEnumerable<string>));
            converter.Write(writer, value.Select(container => container.Acronym), options);
        }
    }
}
