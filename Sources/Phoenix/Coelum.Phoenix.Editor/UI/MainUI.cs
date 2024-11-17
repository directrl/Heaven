using System.Text;
using Coelum.Debug;
using Coelum.ECS.Serialization;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Systems;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using NativeFileDialog.Extended;

namespace Coelum.Phoenix.Editor.UI {
	
	// TODO option to run the scene as a regular scene in a new window
	public class MainUI : ImGuiUI {
		
		public bool TargetSceneUpdate = false;
		public bool TargetSceneUIRender = false;
		public bool ShowDebugUI = Debugging.Enabled;

		public MainUI(PhoenixScene scene) : base(scene) { }

		public override void Render(float delta) {
			ImGui.BeginMainMenuBar();
			{
				if(ImGui.BeginMenu("File")) {
					if(ImGui.MenuItem("Import")) {
						var filePath = NFD.OpenDialog(Environment.CurrentDirectory, new() {
							["json File"] = "json"
						});

						if(!string.IsNullOrWhiteSpace(filePath)) {
							using(var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
								foreach(var simulation in SimulationManager
									        .GetSimulationsByScene(EditorApplication.TargetScene)) {
									
									simulation.GetStore()?.Clear();
								}
								EditorApplication.TargetScene.Import(stream);
							}
						
							// reset output scenes
							// EditorApplication.MainScene.EditorView.OnLoad(EditorApplication.MainWindow);
							// EditorApplication.MainScene.OutputView.OnLoad(EditorApplication.MainWindow);

							if(EditorApplication.TargetScene.PrimaryCamera is not null) {
								EditorApplication.MainScene.OutputView.OutputViewport.Camera =
									(Camera3D) EditorApplication.TargetScene.PrimaryCamera;
							}
						}
					}
					
					if(ImGui.MenuItem("Export")) {
						var filePath = NFD.SaveDialog(Environment.CurrentDirectory, "scene.json");

						if(!string.IsNullOrWhiteSpace(filePath)) {
							using(var stream = new MemoryStream()) {
								EditorApplication.TargetScene.Export(stream);

								string json = Encoding.UTF8.GetString(stream.ToArray());
								File.WriteAllText(filePath, json);
							}
						}
					}
					
					ImGui.EndMenu();
				}
				
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
			
			ImGui.ShowDemoWindow();
		}
	}
}