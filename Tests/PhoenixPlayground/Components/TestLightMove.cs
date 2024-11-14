using System.Text.Json;
using Coelum.ECS;

namespace PhoenixPlayground.Components {

	public class TestLightMove : INodeComponent {

		public Node? Owner { get; set; }

		public void Export(Utf8JsonWriter writer) { }
	}
}