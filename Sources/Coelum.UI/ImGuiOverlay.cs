using Coelum.Configuration;
using Coelum.Graphics.Phoenix.OpenGL;
using Coelum.Graphics.Phoenix.Scene;
using ImGuiNET;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using static Coelum.Graphics.Phoenix.OpenGL.GlobalOpenGL;

namespace Coelum.UI {
	
	public class ImGuiOverlay : OverlayUI {

		private readonly string _iniPath;
		
		protected ImGuiController Controller { get; }
		
		public ImGuiOverlay(PhoenixSceneBase scene) {
			if(scene.Window?.Input == null) {
				throw new ArgumentException("Scene must already have a window with input assigned! "
					+ "Are you creating an overlay in the constructor instead of in Load()?");
			}
			
			_iniPath = scene.Options.ConfigFile.FullName.Replace(GameOptions.FORMAT, ".imgui.ini");
			
			Controller = new(Gl, scene.Window.SilkImpl, scene.Window.Input, onConfigureIO: () => {
				ImGuiManager.SetDefaults(ImGui.GetIO(), _iniPath);
			});

			scene.Render += delta => {
				scene.Window.SilkImpl.MakeCurrent();
				Controller.MakeCurrent();
				ImGui.SetCurrentContext(Controller.Context);
				OnRender(delta);
			};
			
			scene.Unload += () => {
				var ctx = Controller.Context;
				ImGui.SetCurrentContext(ctx);
				ImGui.SaveIniSettingsToDisk(_iniPath);
			};
		}

		public override void OnRender(float delta, params dynamic[] args) {
			Controller.Update(delta);
			base.OnRender(delta, args);
			Controller.Render();
		}
	}
}