using Coelum.ECS;
using Coelum.ECS.Prefab;

namespace EditorPlayground.Prefabs {
	
	[Prefab("two")]
	public class TwoPrefab : Node, IPrefab {

		public Node Create(NodeRoot root) {
			return new TwoPrefab();
		}
	}
}