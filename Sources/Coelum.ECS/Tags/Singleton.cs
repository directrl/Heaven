using System.Text.Json;
using System.Text.Json.Nodes;

namespace Coelum.ECS.Tags {

	public class Singleton : INodeComponent {

		public Node? Owner { get; set; }

		public void Serialize(string name, Utf8JsonWriter writer) { }

		public INodeComponent Deserialize(JsonNode node) {
			return new Singleton();
		}
	}
}