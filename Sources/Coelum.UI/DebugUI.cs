using Coelum.Graphics.Scene;
using ImGuiNET;

namespace Coelum.UI {
	
	public class DebugUI : ImGuiOverlay {
		
		public delegate void AdditionalInfoEventHandler(float delta, params dynamic[] args);

		private readonly SceneBase _scene;
		public event AdditionalInfoEventHandler? AdditionalInfo;

		public DebugUI(SceneBase scene) : base(scene) {
			_scene = scene;
		}

		public override void OnRender(float delta, params dynamic[] args) {
			Controller.Update(delta);

			if(ImGui.Begin("Standard Debug UI", ImGuiWindowFlags.AlwaysAutoResize)) {
				float updMs = (_scene.Window?.UpdateDelta ?? 0) * 1000;
				float fxUpdMs = (_scene.Window?.FixedUpdateDelta ?? 0) * 1000;
				float rndMs = (_scene.Window?.RenderDelta ?? 0) * 1000;
					
				ImGui.Text($"Update delta: {updMs:F2}ms ({(1000 / updMs):F2} FPS)");
				ImGui.Text($"FixedUpdate delta: {fxUpdMs:F2}ms ({(1000 / fxUpdMs):F2} FPS)");
				ImGui.Text($"Render delta: {rndMs:F2}ms ({(1000 / rndMs):F2})");
				
				AdditionalInfo?.Invoke(delta, args);
			}
		
			Controller.Render();
		}
	}
}