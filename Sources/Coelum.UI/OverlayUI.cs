using Coelum.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.UI {

	public delegate void OverlayRender(float delta, params dynamic[] args);
	
	public class OverlayUI {

		protected GL? GL { get; private set; }
		public event OverlayRender? Render;

		public OverlayUI(GL? gl = null) {
			if(gl != null) GL = gl;
			else GL = GLManager.Current;
		}

		public virtual void OnRender(float delta, params dynamic[] args) {
			Render?.Invoke(delta, args);
		}
	}
}