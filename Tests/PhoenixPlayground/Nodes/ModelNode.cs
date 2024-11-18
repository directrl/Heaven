using Coelum.ECS;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Components;
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
			AddComponent<Renderable>(new ModelRenderable(model));
			AddComponent<Transform>(new Transform3D());
		}
	}
}