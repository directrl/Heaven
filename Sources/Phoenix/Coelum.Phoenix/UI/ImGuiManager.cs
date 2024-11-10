using System.Text;
using Coelum.Configuration;
using Coelum.Phoenix.Extensions.ImGui;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.UI {
	
	public class ImGuiManager {

		private static Dictionary<PhoenixScene, ImGuiController> _controllers = new();

		public static ImGuiController CreateController(PhoenixScene scene) {
			if(_controllers.TryGetValue(scene, out var controller)) {
				return controller;
			}
			
			string iniPath = scene.Options.ConfigFile.FullName.Replace(GameOptions.FORMAT, ".imgui.ini");
			
			controller = new(Gl, scene.Window.SilkImpl, scene.Window.Input, onConfigureIO: () => {
				SetDefaults(ImGui.GetIO(), iniPath);
			});
			_controllers[scene] = controller;

			scene.Unload += () => {
				controller.MakeCurrent();
				ImGui.SaveIniSettingsToDisk(iniPath);
			};
			
			return controller;
		}
		
		private unsafe static void SetDefaults(ImGuiIOPtr ioPtr, string iniPath = "imgui.ini") {
			var io = ioPtr.Handle;
			io->ConfigFlags |= ImGuiConfigFlags.DockingEnable;

			byte[] iniPathBytes = Encoding.UTF8.GetBytes(iniPath);
			byte[] c_str = new byte[iniPathBytes.Length + 1];
			
			Array.Copy(iniPathBytes, c_str, iniPathBytes.Length);
			c_str[^1] = 0;
			
			fixed(byte* ptr = c_str) {
				io->IniFilename = ptr;
			}

			io->WantSaveIniSettings = 1;
		}
	}
}