using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class DebugPhysicsRenderSystem : EcsSystem {

		public ShaderProgram Shader { get; set; }

		public override string Name => "Physics Debug Render";
		public override SystemPhase Phase => SystemPhase.RENDER_POST;

		public DebugPhysicsRenderSystem(ShaderProgram shader) {
			Shader = shader;
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
		#if DEBUG
			DebugShapeRenderer.Render(Shader);
		#endif
		}
	}
}