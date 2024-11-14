using System.Text.Json;

namespace Coelum.ECS.Tags {

	public class Singleton : INodeComponent {

		public Node? Owner { get; set; }

		public void Export(Utf8JsonWriter writer) { }
	}
}