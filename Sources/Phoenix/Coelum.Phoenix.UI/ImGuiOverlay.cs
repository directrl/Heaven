using Coelum.Configuration;
using Coelum.Phoenix.OpenGL;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.UI {
	
	public class ImGuiOverlay : OverlayUI {
		
		protected ImGuiContextPtr Context { get; }
		
		public ImGuiOverlay(PhoenixScene scene) {
			if(scene.Window?.Input == null) {
				throw new ArgumentException("Scene must already have a window with input assigned! "
					+ "Are you creating an overlay in the constructor instead of in Load()?");
			}
			
			var iniPath = scene.Options.ConfigFile.FullName.Replace(GameOptions.FORMAT, ".imgui.ini");
			Context = ImGuiManager.Setup(scene.Window, iniPath);

			scene.Render += delta => {
				OnRender(delta);
			};
			
			scene.Unload += () => {
				ImGui.SetCurrentContext(Context);
				ImGui.SaveIniSettingsToDisk(iniPath);
			};
		}

		public override void OnRender(float delta, params dynamic[] args) {
			ImGui.SetCurrentContext(Context);
			ImGuiManager.Begin();
			
			base.OnRender(delta, args);
			
			ImGuiManager.End();
		}
	}
}