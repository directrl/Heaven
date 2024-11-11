using Coelum.Phoenix.Extensions.ImGui;

namespace Coelum.Phoenix.UI {
	
	public abstract class ImGuiUI : OverlayUI {

		protected ImGuiController Controller { get; }

		public ImGuiUI(PhoenixScene scene) : base(scene) {
			Controller = ImGuiManager.CreateController(scene);
		}
	}
}