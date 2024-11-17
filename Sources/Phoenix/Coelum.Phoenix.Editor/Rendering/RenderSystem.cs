using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ECS.Systems;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Editor.Rendering {
	
	public class RenderSystem : ObjectRenderSystem {

		public RenderSystem(ShaderProgram shader) : base(shader) {
			Action = ActionImpl;
		}

		protected new void ActionImpl(NodeRoot root, float delta) {
			root.Query<Renderable, Transform>()
			    .Each((node, renderable, transform) => {
				    _shader.SetUniform("raycast_hit",
				                       node == EditorApplication
				                               .MainScene.EditorViewUI.RayCast.Result.PhysicsNode);
				    
				    _shader.SetUniform("current_light", node.HasComponent<Light>());
				    _shader.SetUniform("model", transform.GlobalMatrix);
				    renderable.Render(_shader);
			    })
			    .Execute();
		}
	}
}