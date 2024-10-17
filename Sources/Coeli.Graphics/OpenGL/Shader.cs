using Coeli.Resources;
using Serilog;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public class Shader {
		
		public ShaderType Type { get; }
		public string Code { get; private set; }

		public Shader(ShaderType type, string code) {
			Type = type;
			Code = code;
		}
		
		public Shader(ShaderType type, Resource resource) {
			Type = type;

			string? code = resource.ReadString();
			Code = code ?? throw new ArgumentException("Invalid shader resource", nameof(resource));
		}

		public void Preprocess(ResourceManager resourceManager) {
			Log.Verbose("Starting shader preprocessing");
			PreprocessIncludes(resourceManager);
			Log.Verbose("Finished preprocessing");
		}

		private void PreprocessIncludes(ResourceManager resourceManager) {
			foreach(string line in Code.Replace("\r\n", "\n").Split('\n')) {
				const string includeToken = "//$include ";

				if(line.StartsWith(includeToken)) {
					string includeName = line.Replace(includeToken, "");
					var resource = resourceManager[Resource.Type.SHADER, includeName];

					string? code = resource.ReadString();
					if(code == null) {
						throw new PreprocessingException("Could not read resource for included shader");
					}

					Code = Code.Replace(line, code);
					
					Log.Verbose($"Included shader [{includeName}]");
				}
			}
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

		// public class Overlay {
		// 	
		// 	public string Name { get; }
		// 	public ShaderType Type { get; }
		// 	public ShaderPass Pass { get; }
		// 	
		// 	public Overlay()
		// }

		public record Overlay(string Name, string Path, ShaderType Type, ShaderPass Pass);
	}

	public enum ShaderPass {
		
		Pre,
		Post
	}
}