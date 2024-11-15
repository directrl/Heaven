using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public class Vector4Serializer : ISerializable<Vector4> {
		
		public Vector4 Value { get; init; }

		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(name);
			{
				writer.WriteNumber("x", Value.X);
				writer.WriteNumber("y", Value.Y);
				writer.WriteNumber("z", Value.Z);
				writer.WriteNumber("w", Value.W);
			}
			writer.WriteEndObject();
		}
		
		public Vector4 Deserialize(JsonNode node) {
			return new(
				node["x"].GetValue<float>(),
				node["y"].GetValue<float>(),
				node["z"].GetValue<float>(),
				node["w"].GetValue<float>()
			);
		}
	}
	
	public static class Vector4SerializerExtensions {

		public static Vector4Serializer Serializer(this Vector4 vec) {
			return new() {
				Value = vec
			};
		}
	}
}