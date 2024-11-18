using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ECS.Systems;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Editor.Rendering {
	
	public class RenderSystem : ObjectRenderSystem {

		public RenderSystem(ShaderProgram shader) : base(shader) {
			Query = new ComponentQuery<Renderable, Transform>(Phase, QueryAction);
		}

		private new void QueryAction(NodeRoot root, Renderable renderable, Transform transform) {
			var node = renderable.Owner!;
			
			_shader.SetUniform("raycast_hit",
			                   node == EditorApplication
			                           .MainScene.EditorViewUI.RayCast.Result.PhysicsNode);
				    
			_shader.SetUniform("current_light", node.HasComponent<Light>());
			_shader.SetUniform("model", transform.GlobalMatrix);
			renderable.Render(_shader);
		}
	}
}