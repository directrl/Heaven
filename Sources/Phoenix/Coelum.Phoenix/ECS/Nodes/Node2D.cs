using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class Node2D : Node {

		public Node2D() {
			AddComponent<Transform>(new Transform2D());
		}
	}
}