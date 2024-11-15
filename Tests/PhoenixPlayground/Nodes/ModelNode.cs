using Coelum.ECS;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ModelLoading;

namespace PhoenixPlayground.Nodes {
	
	public class ModelNode : Node {

		public Model Model {
			get => GetComponent<Renderable, ModelRenderable>().Model;
			set => GetComponent<Renderable, ModelRenderable>().Model = value;
		}

		public ModelNode() {
			AddComponent<Renderable>(new ModelRenderable(ModelLoader.DEFAULT_CUBE));
			AddComponent<Transform>(new Transform3D());
		}

		public ModelNode(Model model) {
			Components = new() {
				{ typeof(Renderable), new ModelRenderable(model) },
				{ typeof(Transform), new Transform3D() }
			};
		}
	}
}