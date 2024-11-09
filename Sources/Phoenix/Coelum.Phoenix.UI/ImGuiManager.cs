using System.Text;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Silk.NET.Core;
using ImGLFW = Hexa.NET.ImGui.Backends.GLFW.ImGuiImplGLFW;
using ImOpenGL = Hexa.NET.ImGui.Backends.OpenGL3.ImGuiImplOpenGL3;

namespace Coelum.Phoenix.UI {
	
	public static class ImGuiManager {

		private static List<IntPtr> _initializedWindows = new();
		private static Dictionary<IntPtr, ImGuiContextPtr> _contexts = new();

		public unsafe static ImGuiContextPtr Setup(SilkWindow window, string iniPath = "imgui.ini") {
			if(_contexts.TryGetValue(window.SilkImpl.Handle, out var ctx)) {
				return ctx;
			}
			
			ctx = ImGui.CreateContext();
			ImGui.SetCurrentContext(ctx);

			var io = ImGui.GetIO();
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

			byte[] iniPathBytes = Encoding.UTF8.GetBytes(iniPath);
			fixed(byte* ptr = iniPathBytes) {
				io.IniFilename = ptr;
			}

			IntPtr windowHandle = window.SilkImpl.Handle;
			
			ImGLFW.SetCurrentContext(ctx);
			if(!ImGLFW.InitForOpenGL(new((GLFWwindow*) windowHandle), true)) {
				throw new PlatformException("Failed to initialize an ImGui context");
			}
			
			ImOpenGL.SetCurrentContext(ctx);
			if(!ImOpenGL.Init("#version 330")) {
				throw new PlatformException("Failed to initialize an ImGui context");
			}

			_contexts[windowHandle] = ctx;
			return ctx;
		}

		public static void Begin() {
			ImOpenGL.NewFrame();
			ImGLFW.NewFrame();
			ImGui.NewFrame();
		}

		public static void End() {
			ImGui.Render();
			ImOpenGL.RenderDrawData(ImGui.GetDrawData());
		}
	}
}