using System.Numerics;
using System.Runtime.InteropServices;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Texture;

namespace Coeli.Graphics {
	
	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public struct Material {

		[FieldOffset(0)] public Vector4 Color;
		
		[FieldOffset(16)] public Texture<Vector2> Texture;
		[FieldOffset(24)] public int TextureLayer;
		
		// TODO blending and transparency

		public Material() {
			Color = new(1, 1, 1, 1);
			Texture = Texture2D.DefaultTexture;
			TextureLayer = -1;
		}

		public void Load(ShaderProgram shader) {
			shader.SetUniform("material.color", Color);
			shader.SetUniform("material.texLayer", TextureLayer);
			
			Texture.Bind();
		}
	}
}