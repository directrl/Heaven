global using static Coelum.Phoenix.GLFW.GlobalGLFW;

using Silk.NET.GLFW;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.GLFW {
	
	public static class GlobalGLFW {

		public static Glfw GlFw { get; private set; }

		static GlobalGLFW() {
			GlFw = Glfw.GetApi();
		}
	}
}