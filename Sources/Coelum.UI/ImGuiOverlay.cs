using Coelum.Configuration;
using Coelum.Graphics.OpenGL;
using Coelum.Graphics.Scene;
using Silk.NET.OpenGL.Extensions.ImGui;

namespace Coelum.UI {
	
	public class ImGuiOverlay : OverlayUI {

		private readonly string _iniPath;
		protected ImGuiController Controller { get; }
		
		public ImGuiOverlay(SceneBase scene) : base(scene.Window?.GL ?? GLManager.Current) {
			if(scene.Window == null) {
				throw new ArgumentException("Scene must already have a window assigned! "
					+ "Are you creating an overlay in the constructor instead of in Load()?");
			}
			
			_iniPath = scene.Options.ConfigFile.FullName.Replace(GameOptions.FORMAT, ".imgui.ini");
			
			// TODO each new controller resets the previous frame meaning you cant have more than 1 overlay per window
			Controller = new(GL, scene.Window.SilkImpl, scene.Window.Input, onConfigureIO: () => {
				ImGuiManager.SetDefaults(ImGuiNET.ImGui.GetIO(), _iniPath);
			});

			scene.Render += (gl, delta) => OnRender(delta);
			scene.Unload += () => {
				var ctx = Controller.Context;
				ImGuiNET.ImGui.SetCurrentContext(ctx);
				ImGuiNET.ImGui.SaveIniSettingsToDisk(_iniPath);
			};
		}

		public override void OnRender(float delta, params dynamic[] args) {
			Controller.Update(delta);
			base.OnRender(delta, args);
			Controller.Render();
		}
	}
}