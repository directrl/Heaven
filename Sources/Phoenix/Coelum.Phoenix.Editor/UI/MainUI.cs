using Coelum.Debug;
using Coelum.Phoenix.ECS.System;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class MainUI : ImGuiUI {
		
		public bool TargetSceneUpdate = false;
		public bool TargetSceneUIRender = false;
		public bool ShowDebugUI = Debugging.Enabled;

		public MainUI(PhoenixScene scene) : base(scene) { }

		public override void Render(float delta) {
			ImGui.BeginMainMenuBar();
			{
				if(ImGui.BeginMenu("Options")) {
					if(ImGui.BeginMenu("Target Scene")) {
						if(ImGui.MenuItem("Update", "", ref TargetSceneUpdate)) { }
						if(ImGui.MenuItem("UI Render", "", ref TargetSceneUIRender)) {
							var uiSystem = EditorApplication.TargetScene.QuerySystem<UISystem>();
							if(uiSystem != null) uiSystem.Enabled = TargetSceneUIRender;
						}
						
						ImGui.EndMenu();
					}

					if(ImGui.MenuItem("Show Debug UI", "", ref ShowDebugUI)) {
						if(ShowDebugUI) {
							foreach(var overlay in Scene.UIOverlays) {
								if(overlay is DebugOverlay) {
									overlay.Visible = true;
									break;
								}
							}
						} else {
							foreach(var overlay in Scene.UIOverlays) {
								if(overlay is DebugOverlay) {
									overlay.Visible = false;
									break;
								}
							}
						}
					}
					
					ImGui.EndMenu();
				}
			}
			ImGui.EndMainMenuBar();
		}
	}
}