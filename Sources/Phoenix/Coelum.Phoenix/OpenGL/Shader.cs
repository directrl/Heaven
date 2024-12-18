using Coelum.Resources;
using Silk.NET.Core;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL {
	
	public class Shader {
		
		internal ShaderOverlay[] _overlays = { };
		internal Dictionary<string, string> _definitions = new();
		
		public ShaderType Type { get; }
		public string Code { get; internal set; }

		public Shader(ShaderType type, string code) {
			Type = type;
			Code = code;
		}
		
		public Shader(ShaderType type, IResource resource) {
			Type = type;

			string? code = resource.ReadString();
			Code = code ?? throw new ArgumentException("Invalid shader resource", nameof(resource));
		}
		
		public uint Compile() {
			if(string.IsNullOrEmpty(Code)) {
				throw new ArgumentNullException("code", "Cannot compile an empty shader!");
			}
			
			uint id = Gl.CreateShader(Type);

			if(id == 0) {
				throw new PlatformException("Could not create a GL shader");
			}
			
			Gl.ShaderSource(id, Code);
			Gl.CompileShader(id);

			if(Gl.GetShader(id, GLEnum.CompileStatus) == 0) {
				Console.WriteLine("===== FAULTING SHADER CODE BEGIN =====");

				{
					string[] c = Code.Split("\n");

					for(int i = 0; i < c.Length; i++) {
						Console.WriteLine($"{i + 1}: {c[i]}");
					}
				}
				
				Console.WriteLine("===== FAULTING SHADER CODE END =====");
				Console.WriteLine();
				
				throw new CompilationException(id);
			}

			return id;
		}
		
		public class CompilationException : Exception {

			public CompilationException(uint id)
				: base($"Error occured during shader compilation: {Gl.GetShaderInfoLog(id)}") { }
		}

		public class PreprocessingException : Exception {
			
			public PreprocessingException(string message)
				: base($"Error occured during shader preprocessing: {message}") { }
		}
	}

	public class ShaderPass {

		public static readonly ShaderPass COLOR_PRE = new("COLOR_PRE");
		public static readonly ShaderPass COLOR_PRE_STAGE2 = new("COLOR_PRE_STAGE2"); // TODO priority system
		public static readonly ShaderPass COLOR_POST = new("COLOR_POST");
		
		public static readonly ShaderPass POSITION_PRE = new("POSITION_PRE");
		public static readonly ShaderPass POSITION_POST = new("POSITION_POST");
		
		public static readonly ShaderPass RETURN = new("RETURN");
		public static readonly ShaderPass RETURN_STAGE2 = new("RETURN_STAGE2");
		
		internal string Name { get; }

		internal ShaderPass(string name) {
			Name = name;
		}
	}
}