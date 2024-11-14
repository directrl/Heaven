using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public class Vector3Serializer : ISerializable<Vector3> {
		
		public Vector3 Value { get; init; }

		public void Export(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(name);
			{
				writer.WriteNumber("x", Value.X);
				writer.WriteNumber("y", Value.Y);
				writer.WriteNumber("z", Value.Z);
			}
			writer.WriteEndObject();
		}
		
		public Vector3 Import(JsonNode node) {
			return new(
				node["x"].GetValue<float>(),
				node["y"].GetValue<float>(),
				node["z"].GetValue<float>()
			);
		}
	}
	
	public static class Vector3SerializerExtensions {

		public static Vector3Serializer Serializer(this Vector3 vec) {
			return new() {
				Value = vec
			};
		}
	}
}