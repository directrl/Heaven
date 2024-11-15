using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public interface ISerializable<T> {

		public void Serialize(string name, Utf8JsonWriter writer);
		public T Deserialize(JsonNode node);
	}
}