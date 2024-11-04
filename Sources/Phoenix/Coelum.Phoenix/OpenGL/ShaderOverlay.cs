using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL {
	
	// TODO overlay parameters/variables
	public abstract class ShaderOverlay {

		public abstract string Name { get; }
		public abstract string Path { get; }

		public virtual bool HasHeader { get; } = true;
		public virtual bool HasCall { get; } = true;
		
		public abstract ShaderType Type { get; }
		public abstract ShaderPass Pass { get; }

		public abstract ResourceManager ResourceManager { get; }
		
		/// <summary>
		/// Gets called when the overlay is included into the shader program code for the first time.
		/// This can be used for checking if some dependencies are met.
		/// </summary>
		/// <param name="shader">The shader program the overlay is being included into</param>
		public virtual void Include(ShaderProgram shader) { }
		
		/// <summary>
		/// Gets called each time the shader program gets bound.
		/// This can be used for setting constant uniforms or default values.
		/// </summary>
		/// <param name="shader">The shader program the overlay is included in</param>
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