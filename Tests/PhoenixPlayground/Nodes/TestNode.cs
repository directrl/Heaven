using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;

namespace PhoenixPlayground.Nodes {
	
	public class TestNode : Node {

		public TestNode() {
			Components = new() {
				{ typeof(Renderable), new ModelRenderable(ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"])) },
				{ typeof(Transform), new Transform3D() }
			};
		}
	}
}