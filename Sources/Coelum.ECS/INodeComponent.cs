using System.Text.Json;

namespace Coelum.ECS {

	public interface INodeComponent {
		
		public Node? Owner { get; set; }

		public void Export(Utf8JsonWriter writer);
	}
}