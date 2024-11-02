using Coelum.ECS;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Light : INodeComponent {

		public Node? Owner { get; set; }
	}
}