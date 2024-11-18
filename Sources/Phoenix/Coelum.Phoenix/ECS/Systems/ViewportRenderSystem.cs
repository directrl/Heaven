using Coelum.ECS;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class ViewportRenderSystem : EcsSystem {
		
		private CameraMatrices _ubo;
		private Action<float> _renderAction;
		
		public ViewportRenderSystem(ShaderProgram shader, Action<float> renderAction)
			: base("Viewport Render", SystemPhase.RENDER) {

			_ubo = shader.CreateBufferBinding<CameraMatrices>();
			_renderAction = renderAction;
			
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.QueryChildren<Viewport>()
			    .Each(viewport => {
				    if(!viewport.Enabled) return;
				    
				    viewport.Framebuffer.Bind();
				    viewport.Camera.Load(_ubo);
				    _ubo.Upload();
				    _renderAction.Invoke(delta);
			    })
			    .Execute();
		}
	}
}