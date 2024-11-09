using Coelum.Core;
using Coelum.ECS;
using Coelum.ECS.Prefab;
using Coelum.Phoenix;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using EditorPlayground.Nodes;

namespace EditorPlayground.Prefabs {
	
	[Prefab("CRT")]
	public class CrtPrefab : Node, IPrefab {

		public Node Create(NodeRoot root) {
			return new ModelNode(ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "crt.glb"]));
		}
	}
}