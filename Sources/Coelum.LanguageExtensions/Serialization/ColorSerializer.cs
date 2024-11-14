using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public class ColorSerializer : ISerializable<Color> {

		public Color Value { get; init; }

		public void Export(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(name);
			{
				writer.WriteNumber("a", Value.A);
				writer.WriteNumber("r", Value.R);
				writer.WriteNumber("g", Value.G);
				writer.WriteNumber("b", Value.B);
			}
			writer.WriteEndObject();
		}
		
		public Color Import(JsonNode node) {
			return Color.FromArgb(
				node["a"].GetValue<byte>(),
				node["r"].GetValue<byte>(),
				node["g"].GetValue<byte>(),
				node["b"].GetValue<byte>()
			);
		}
	}

	public static class ColorSerializerExtensions {

		public static ColorSerializer Serializer(this Color color) {
			return new() {
				Value = color
			};
		}
	}
}