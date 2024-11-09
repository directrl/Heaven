using Coelum.ECS;
using Coelum.ECS.Prefab;

namespace EditorPlayground.Prefabs {
	
	[Prefab("three")]
	public class ThreePrefab : Node, IPrefab {

		public Node Create(NodeRoot root) {
			return new ThreePrefab();
		}
	}
}