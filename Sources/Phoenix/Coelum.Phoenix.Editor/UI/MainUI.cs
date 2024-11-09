using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class MainUI : ImGuiOverlay {

		public MainUI(PhoenixScene scene) : base(scene) {
			Render += (delta, args) => {
				if(ImGui.BeginMainMenuBar()) {
					ImGui.EndMainMenuBar();
				}
			};
		}
	}
}