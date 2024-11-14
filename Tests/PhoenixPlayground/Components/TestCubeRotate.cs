using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;

namespace PhoenixPlayground.Components {
	
	public class TestCubeRotate : INodeComponent {

		public Node? Owner { get; set; }

		public void Serialize(string name, Utf8JsonWriter writer) { }

		public INodeComponent Deserialize(JsonNode node) {
			return new TestCubeRotate();
		}
	}
}