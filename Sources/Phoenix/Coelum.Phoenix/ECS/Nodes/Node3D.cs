using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class Node3D : Node {

		public Node3D() {
			AddComponent<Transform>(new Transform3D());
		}
	}
}