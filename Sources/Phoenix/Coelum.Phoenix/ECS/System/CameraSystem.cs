using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.System {
	
	// TODO to remove
	[Obsolete]
	public class CameraSystem : EcsSystem {
		
		private readonly CameraMatrices _ubo;

		public CameraSystem(ShaderProgram shader) : base("Camera Render") {
			_ubo = shader.CreateBufferBinding<CameraMatrices>();
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Component.Camera>()
			    .Each((node, camera) => {
				    if(!camera.TheCamera.Current) return;
				    camera.Load(_ubo);
			    })
			    .Execute();
			
			_ubo.Upload();
		}
	}
}