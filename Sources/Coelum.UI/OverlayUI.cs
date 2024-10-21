namespace Coelum.UI {

	public delegate void OverlayRender(float delta, params dynamic[] args);
	
	public class OverlayUI {

		public event OverlayRender? Render;

		public virtual void OnRender(float delta, params dynamic[] args) {
			Render?.Invoke(delta, args);
		}
	}
}