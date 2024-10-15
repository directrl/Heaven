using System.Numerics;
using Coeli.Graphics.OpenGL;

namespace Coeli.Graphics {
	
	public struct Material {

		public static readonly Material DEFAULT_MATERIAL = new Material {
			Color = new(1.0f, 1.0f, 1.0f, 1.0f)
		};
		
		public Vector4 Color { get; set; }

		public void Load(ShaderProgram shader) {
			shader.SetUniform("material.color", Color);
		}
	}
}