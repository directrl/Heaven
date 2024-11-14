using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.LanguageExtensions.Serialization {
	
	public interface ISerializable<T> {
		
		public T Value { get; init; }

		public void Export(string name, Utf8JsonWriter writer);
		public T Import(JsonNode node);
	}
}