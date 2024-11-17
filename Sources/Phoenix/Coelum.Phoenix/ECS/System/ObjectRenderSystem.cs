using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	public class ObjectRenderSystem : EcsSystem {

		protected ShaderProgram _shader;

		public ObjectRenderSystem(ShaderProgram shader) : base("Object Render") {
			_shader = shader;
			Action = ActionImpl;
		}

		protected void ActionImpl(NodeRoot root, float delta) {
			root.Query<Renderable, Transform>()
			    .Each((node, renderable, transform) => {
				    _shader.SetUniform("current_light", node.HasComponent<Light>());
				    _shader.SetUniform("model", transform.GlobalMatrix);
				    renderable.Render(_shader);
			    })
			    .Execute();
		}
	}
}