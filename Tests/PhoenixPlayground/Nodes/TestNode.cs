using Coelum.Node;
using Coelum.Phoenix;
using Coelum.Phoenix.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using PhoenixPlayground;
using Renderable = Coelum.Phoenix.Node.Component.Renderable;

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