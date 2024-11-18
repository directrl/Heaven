using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class ObjectRenderSystem : ChildQuerySystem {

		protected ShaderProgram _shader;

		public override string Name => "Object Render";
		public override SystemPhase Phase => SystemPhase.RENDER_POST;

		public ObjectRenderSystem(ShaderProgram shader) {
			_shader = shader;

			Query = new ComponentQuery<Renderable, Transform>(Phase, QueryAction);
		}

		private void QueryAction(NodeRoot root, Renderable renderable, Transform transform) {
			var node = renderable.Owner!;
			
			_shader.SetUniform("current_light", node.HasComponent<Light>());
			_shader.SetUniform("model", transform.GlobalMatrix);
			renderable.Render(_shader);
		}
	}
}