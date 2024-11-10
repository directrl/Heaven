namespace Coelum.Phoenix.UI {
	
	public abstract class OverlayUI {

		protected PhoenixScene Scene { get; }

		public OverlayUI(PhoenixScene scene) {
			Scene = scene;
		}
		
		public abstract void Render(float delta);
	}
}