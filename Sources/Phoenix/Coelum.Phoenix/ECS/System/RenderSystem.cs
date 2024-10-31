using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	public class RenderSystem : EcsSystem {

		private ShaderProgram _shader;

		public RenderSystem(ShaderProgram shader) : base("Render") {
			_shader = shader;
			Action = Invoke;
		}

		private void Invoke(NodeRoot root, float delta) {
			root.Query<Renderable, Transform>()
			    .Each((node, renderable, transform) => {
				    if(node.Name == "meow") Console.WriteLine("meowwww");
				    
				    _shader.SetUniform("model", transform.GlobalMatrix);
				    renderable.Render(_shader);
			    })
			    .Execute();
		}
	}
}