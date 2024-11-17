using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Components {
	
	public class ModelRenderable : Renderable {

		public Node? Owner { get; set; }
		public Model Model { get; set; }
		
		public ModelRenderable() { }
		
		public ModelRenderable(Model model) {
			Model = model;
		}

		public void Render(ShaderProgram shader) {
			Model.Render(shader);
		}
	}
}