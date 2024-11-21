using Coelum.Phoenix;

namespace PhoenixPlayground.Scenes {
	
	public class FramebufferTest : LightingTest {

		private Framebuffer _fbo;

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			// disable any previous viewports from LightingTest
			QueryChildren<Viewport>()
				.Each(viewport => {
					viewport.Enabled = false;
				})
				.Execute();
			
			_fbo = new Framebuffer(new(window.Framebuffer.Size.X / 5, window.Framebuffer.Size.Y / 5), window) {
				AutoResize = true,
				AutoResizeFactor = 0.2f
			};
			
			Add(new Viewport(PrimaryCamera, _fbo));
		}

		// protected override void DoRender(float delta) {
		// 	base.DoRender(delta);
		// 	
		// 	Window.Framebuffer.Bind();
		// 	_fbo.Render();
		// }
	}
}