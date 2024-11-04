using Coelum.Phoenix;

namespace PhoenixPlayground.Scenes {
	
	public class FramebufferTest : LightingTest {

		public override void OnLoad(SilkWindow window) {
			Framebuffer = new(window.FramebufferWidth / 2, window.FramebufferHeight / 2) {
				AutoResize = true,
				AutoResizeFactor = 0.5f
			};

			
			base.OnLoad(window);
		}
	}
}