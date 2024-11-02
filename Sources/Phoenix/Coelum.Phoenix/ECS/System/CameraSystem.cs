using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	internal class CameraSystem : EcsSystem {
		
		private ShaderProgram _shader;

		public CameraSystem(ShaderProgram shader) : base("Camera Render") {
			_shader = shader;
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<CameraRenderable>()
			    .Each((node, renderable) => {
				    if(!renderable.Camera.Current) return;
				    renderable.Render(_shader);
			    })
			    .Execute();
		}
	}
}