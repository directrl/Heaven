using System.Text;
using Coelum.Debug;
using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Silk.NET.Core;
using ImGLFW = Hexa.NET.ImGui.Backends.GLFW.ImGuiImplGLFW;
using ImOpenGL = Hexa.NET.ImGui.Backends.OpenGL3.ImGuiImplOpenGL3;

namespace Coelum.Phoenix.UI {
	
	// TODO this doesnt work with:
	// - multiple windows
	// - multiple contexts
	// - multiple imgui calls per context
	public static class ImGuiManager {

		private static Dictionary<IntPtr, ImGuiContextPtr> _contexts = new();
		private static Dictionary<ImGuiContextPtr, bool> _contextStates = new();

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
			_contextStates[ctx] = false;
			return ctx;
		}

		public static void Begin(ImGuiContextPtr ctx) {
			ImGui.SetCurrentContext(ctx);
			
			if(_contextStates[ctx]) return;
			
			ImOpenGL.NewFrame();
			ImGLFW.NewFrame();
			ImGui.NewFrame();
			
			_contextStates[ctx] = true;
		}

		public static void End(ImGuiContextPtr ctx) {
			ImGui.SetCurrentContext(ctx);
			
			if(!_contextStates[ctx]) return;
			
			ImGui.Render();
			ImOpenGL.RenderDrawData(ImGui.GetDrawData());

			_contextStates[ctx] = false;
		}
	}
}