using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class MainUI : ImGuiScene {

		public MainUI() : base("editor-ui_main") {
			
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			//_ = new DebugUI(EditorApplication.TargetScene);
		}

		public override void RenderUI(float delta) {
			base.RenderUI(delta);

			if(ImGui.BeginMainMenuBar()) {
				ImGui.EndMainMenuBar();
			}
		}
	}
}