using Coelum.ECS;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Component;

namespace EditorPlayground.Nodes {
	
	public class ModelNode : Node {

		public ModelNode(Model model) {
			Components = new() {
				{ typeof(Renderable), new ModelRenderable(model) },
				{ typeof(Transform), new Transform3D() }
			};
		}
	}
}