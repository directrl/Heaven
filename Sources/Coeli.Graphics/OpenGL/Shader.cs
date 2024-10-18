using Coeli.Resources;
using Serilog;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public class Shader {
		
		public ShaderType Type { get; }
		public string Code { get; internal set; }
		
		public Overlay[] Overlays { get; internal set; }

		public Shader(ShaderType type, string code) {
			Type = type;
			Code = code;
		}
		
		public Shader(ShaderType type, Resource resource) {
			Type = type;

			string? code = resource.ReadString();
			Code = code ?? throw new ArgumentException("Invalid shader resource", nameof(resource));
		}
		
		public uint Compile(GL gl) {
			if(string.IsNullOrEmpty(Code)) {
				throw new ArgumentNullException("code", "Cannot compile an empty shader!");
			}
			
			uint id = gl.CreateShader(Type);

			if(id == 0) {
				throw new PlatformException("Could not create a GL shader");
			}
			
			gl.ShaderSource(id, Code);
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

		public class PreprocessingException : Exception {
			
			public PreprocessingException(string message)
				: base($"Error occured during shader preprocessing: {message}") { }
		}

		public record Overlay(string Name, string Path,
		                      ShaderType Type, ShaderPass Pass,
		                      ResourceManager resources) {
			
			public virtual void Load(ShaderProgram shader) { }

			public string GetExtension() {
				switch(Type) {
					case ShaderType.FragmentShader:
						return "frag";
					case ShaderType.VertexShader:
						return "vert";
					default:
						throw new ArgumentException($"Unknown shader overlay type {Type}");
				}
			}
		}
	}

	public class ShaderPass {

		public static readonly ShaderPass COLOR_PRE = new("COLOR_PRE");
		public static readonly ShaderPass COLOR_POST = new("COLOR_POST");
		
		internal string Name { get; }

		internal ShaderPass(string name) {
			Name = name;
		}
	}
}