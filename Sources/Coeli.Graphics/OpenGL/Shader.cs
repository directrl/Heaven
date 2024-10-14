using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public record Shader(ShaderType type, string code) {

		public uint Compile(GL gl) {
			if(string.IsNullOrEmpty(code)) {
				throw new ArgumentNullException("code", "Cannot compile an empty shader!");
			}
			
			uint id = gl.CreateShader(type);

			if(id == 0) {
				throw new PlatformException("Could not create a GL shader");
			}
			
			gl.ShaderSource(id, code);
			gl.CompileShader(id);

			if(gl.GetShader(id, GLEnum.CompileStatus) == 0) {
				throw new CompilationException(gl, id);
			}

			return id;
		}
		
		public class CompilationException : Exception {

			public CompilationException(GL gl, uint id)
				: base($"Error occured during shader compilation: {gl.GetShaderInfoLog(id)}") { }
		}
	}
}