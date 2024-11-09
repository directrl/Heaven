using Coelum.ECS;
using Coelum.ECS.Prefab;

namespace EditorPlayground.Prefabs {
	
	[Prefab("one")]
	public class OnePrefab : Node, IPrefab {

		public Node Create(NodeRoot root) {
			return new OnePrefab();
		}
	}
}