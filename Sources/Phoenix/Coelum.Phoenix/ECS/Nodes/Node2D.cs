using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class Node2D : Node {

		public Node2D() {
			AddComponent(new Transform2D());
		}
	}
}