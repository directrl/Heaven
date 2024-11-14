using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public class Vector2Serializer : ISerializable<Vector2> {
		
		public Vector2 Value { get; init; }

		public void Export(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(name);
			{
				writer.WriteNumber("x", Value.X);
				writer.WriteNumber("y", Value.Y);
			}
			writer.WriteEndObject();
		}
		
		public Vector2 Import(JsonNode node) {
			return new(
				node["x"].GetValue<float>(),
				node["y"].GetValue<float>()
			);
		}
	}
	
	public static class Vector2SerializerExtensions {

		public static Vector2Serializer Serializer(this Vector2 vec) {
			return new() {
				Value = vec
			};
		}
	}
}