global using static Coelum.Graphics.OpenGL.GlobalOpenGL;

using Silk.NET.OpenGL;

namespace Coelum.Graphics.OpenGL {
	
	public class GlobalOpenGL {

		public static GL Gl { get; private set; }

		public GlobalOpenGL(GL gl) {
			Gl = gl;
		}
	}
}