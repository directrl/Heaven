using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL {
	
	// TODO overlay dependencies
	public interface IShaderOverlay {

		public string Name { get; }
		public string Path { get; }
		
		public bool HasHeader { get; }
		public bool HasCall { get; }
		
		public ShaderType Type { get; }
		public ShaderPass Pass { get; }

		public ResourceManager ResourceManager { get; }

		

		public void Load(ShaderProgram shader);

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