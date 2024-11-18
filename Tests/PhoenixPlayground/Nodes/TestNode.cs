using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;

namespace PhoenixPlayground.Nodes {
	
	public class TestNode : Node {

		public TestNode() {
			AddComponent<Renderable>(new ModelRenderable(ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"])));
			AddComponent<Transform>(new Transform3D());
		}
	}
}