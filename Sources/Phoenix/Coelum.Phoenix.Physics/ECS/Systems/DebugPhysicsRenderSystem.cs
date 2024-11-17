using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class DebugPhysicsRenderSystem : EcsSystem {

		public ShaderProgram Shader { get; set; }
		
		public DebugPhysicsRenderSystem(ShaderProgram shader) : base("Physics Debug Render") {
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