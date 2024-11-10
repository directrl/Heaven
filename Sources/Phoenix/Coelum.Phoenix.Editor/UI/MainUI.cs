using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class MainUI : ImGuiUI {

		public MainUI(PhoenixScene scene) : base(scene) { }

		public override void Render(float delta) {
			if(ImGui.BeginMainMenuBar()) {
				ImGui.EndMainMenuBar();
			}
		}
	}
}