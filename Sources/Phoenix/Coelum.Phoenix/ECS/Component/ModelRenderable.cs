using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class ModelRenderable : INodeComponent, Renderable {

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