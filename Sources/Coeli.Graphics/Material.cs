using System.Numerics;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Texture;

namespace Coeli.Graphics {
	
	public struct Material {

		public static readonly Material DEFAULT_MATERIAL = new Material {
			Color = new(1.0f, 1.0f, 1.0f, 1.0f),
			Texture = Texture2D.DefaultTexture
		};
		
		public Vector4 Color { get; set; }
		public Texture<Vector2> Texture { get; set; } // TODO texture instancing update: i dont think thats possible
		// TODO blending and transparency

		public void Load(ShaderProgram shader) {
			shader.SetUniform("material.color", Color);
			
			Texture.Bind();
		}
	}
}