using System.Text;
using ImGuiNET;

namespace Coelum.UI {
	
	public static class ImGuiManager {

		public unsafe static void SetDefaults(ImGuiIOPtr ioPtr, string iniPath = "imgui.ini") {
			var io = ioPtr.NativePtr;
			io->ConfigFlags |= ImGuiConfigFlags.DockingEnable;

			byte[] pathBytes = Encoding.UTF8.GetBytes(iniPath);
			byte[] terminated = new byte[pathBytes.Length + 1];
			
			Array.Copy(pathBytes, terminated, pathBytes.Length);
			terminated[^1] = 0;
			
			fixed(byte* b = pathBytes) {
				io->IniFilename = b;
			}

			io->WantSaveIniSettings = 1;
		}
	}
}