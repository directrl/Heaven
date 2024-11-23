using System.Runtime.InteropServices;
using System.Text;
using Coelum.Configuration;
using Coelum.Phoenix.Extensions.ImGui;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.UI {
	
	public class ImGuiManager {

		private static Dictionary<PhoenixScene, ImGuiController> _controllers = new();

		public static ImGuiController CreateController(PhoenixScene scene, Action? onConfigureIo = null) {
			if(_controllers.TryGetValue(scene, out var controller)) {
				return controller;
			}
			
			string iniPath = scene.Options.ConfigFile.FullName.Replace(GameOptions.FORMAT, ".imgui.ini");
			
			controller = new(Gl, scene.Window.SilkImpl, scene.Window.Input, onConfigureIO: () => {
				SetDefaults(ImGui.GetIO(), iniPath);
				onConfigureIo?.Invoke();
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
			byte[] cStr = new byte[iniPathBytes.Length + 1];
			
			Array.Copy(iniPathBytes, cStr, iniPathBytes.Length);
			cStr[^1] = 0;
			
			/*
			 * not sure if this is completely necessary, but ImGui might do funky stuff if the IniFilename
			 * gets freed by .NET's GC
			 */
			
			var cStrAddr = Marshal.AllocHGlobal(cStr.Length);
			for(int i = 0; i < cStr.Length; i++) {
				Marshal.WriteByte(cStrAddr + i, cStr[i]);
			}
			
			io->IniFilename = (byte*) cStrAddr;
			io->WantSaveIniSettings = 1;
		}
	}
}