using Coelum.Configuration;
using Coelum.Phoenix.OpenGL;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.UI {
	
	public class ImGuiOverlay : OverlayUI {
		
		public ImGuiContextPtr Context { get; }
		
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

		// TODO merge all this dumb fucking UI shit into the main package
		// TODO and call all the new frame garbage shit in the scene render method
		// TODO by default because this fucking sucks and fucking breaks with more
		// TODO than one overlay so we have to move it somewhere outside of the actual
		// TODO overlay class but fucking where?????
		public override void OnRender(float delta, params dynamic[] args) {
			//ImGuiManager.Begin(Context);
			base.OnRender(delta, args);
			//ImGuiManager.End(Context);
		}
	}
}