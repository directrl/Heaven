global using static Coelum.Phoenix.OpenGL.GlobalOpenGL;

using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL {
	
	public class GlobalOpenGL {

		public static GL Gl;

		public GlobalOpenGL(GL gl) {
			Gl = gl;
		}
	}
}