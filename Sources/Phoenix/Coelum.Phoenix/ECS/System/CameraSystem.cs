using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.System {
	
	internal class CameraSystem : EcsSystem {
		
		private readonly ShaderProgram _shader;
		private readonly CameraMatrices _ubo;

		public CameraSystem(ShaderProgram shader) : base("Camera Render") {
			_shader = shader;
			_ubo = new();
			
			shader.CreateBufferBinding(_ubo);
			
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