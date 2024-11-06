using Hexa.NET.ImGui;
using Hexa.NET.ImGui.Backends.GLFW;
using Silk.NET.Core;

using ImGLFW = Hexa.NET.ImGui.Backends.GLFW.ImGuiImplGLFW;
using ImOpenGL = Hexa.NET.ImGui.Backends.OpenGL3.ImGuiImplOpenGL3;

namespace Coelum.Phoenix.Editor {
	
	public class HexaImGui {

		public unsafe static void Setup(SilkWindow window) {
			var ctx = ImGui.CreateContext();
			ImGui.SetCurrentContext(ctx);

			var io = ImGui.GetIO();
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
			
			ImGLFW.SetCurrentContext(ctx);
			if(!ImGLFW.InitForOpenGL(new((GLFWwindow*) window.SilkImpl.Handle), true)) {
				throw new PlatformException("Failed to initialize an ImGui context");
			}
			
			ImOpenGL.SetCurrentContext(ctx);
			if(!ImOpenGL.Init("#version 330")) {
				throw new PlatformException("Failed to initialize an ImGui context");
			}
		}

		public static void Begin() {
			ImOpenGL.NewFrame();
			ImGLFW.NewFrame();
			ImGui.NewFrame();
		}

		public static void End() {
			ImGui.Render();
			ImGui.EndFrame();
			ImOpenGL.RenderDrawData(ImGui.GetDrawData());
		}
	}
}