using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class ModelRenderable : NodeComponent, Renderable {

		public Model Model { get; set; }
		
		public ModelRenderable(Model model) {
			Model = model;
		}

		public void Render(ShaderProgram shader) {
			Model.Render(shader);
		}
	}
}