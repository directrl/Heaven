using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class ViewportRenderSystem : ChildQuerySystem {
		
		private CameraMatrices _ubo;
		private Action<float> _renderAction;

		public override string Name => "Viewport Render";
		public override SystemPhase Phase => SystemPhase.RENDER;

		public ViewportRenderSystem(ShaderProgram shader, Action<float> renderAction) {
			_ubo = shader.CreateBufferBinding<CameraMatrices>();
			_renderAction = renderAction;

			Query = new ChildrenQuery<Viewport>(Phase, QueryAction);
		}

		private void QueryAction(NodeRoot root, Viewport viewport) {
			if(!viewport.Enabled) return;
			
			viewport.Framebuffer.Bind();
			viewport.Camera.Load(_ubo);
			_ubo.Upload();
			_renderAction.Invoke(root.GetDeltaTime(Phase));
		}
	}
}