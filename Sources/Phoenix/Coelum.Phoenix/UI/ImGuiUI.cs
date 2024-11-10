using Coelum.Phoenix.Extensions.ImGui;

namespace Coelum.Phoenix.UI {
	
	public abstract class ImGuiUI : OverlayUI {

		private ImGuiController _controller;

		public ImGuiUI(PhoenixScene scene) : base(scene) {
			_controller = ImGuiManager.CreateController(scene);
		}
	}
}