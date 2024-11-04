using Coelum.Phoenix;

namespace PhoenixPlayground.Scenes {
	
	public class FramebufferTest : LightingTest {

		public override void OnLoad(SilkWindow window) {
			Framebuffer = new(window.FramebufferWidth / 5, window.FramebufferHeight / 5) {
				AutoResize = true,
				AutoResizeFactor = 1 / 5f
			};
			
			base.OnLoad(window);
		}
	}
}