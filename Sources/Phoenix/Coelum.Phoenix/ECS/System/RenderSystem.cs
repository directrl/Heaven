using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	public class RenderSystem : EcsSystem {

		private ShaderProgram _shader;

		public RenderSystem(ShaderProgram shader) : base("Object Render") {
			_shader = shader;
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Renderable, Transform>()
			    .Each((node, renderable, transform) => {
				    if(_shader.Overlays.ContainsKey(PhongShading.OVERLAYS[0])
				       || _shader.Overlays.ContainsKey(GouraudShading.OVERLAYS[0])) {
					    
					    _shader.SetUniform("current_light", node.HasComponent<Light>());
				    }
				    
				    _shader.SetUniform("model", transform.GlobalMatrix);
				    renderable.Render(_shader);
			    })
			    .Execute();
		}
	}
}