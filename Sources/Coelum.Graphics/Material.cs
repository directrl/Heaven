using System.Numerics;
using System.Runtime.InteropServices;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Coelum.Graphics.Texture;
using ShadingModel = Silk.NET.OpenGL.ShadingModel;

namespace Coelum.Graphics {
	
	// TODO check if the offsets are correct
	// [StructLayout(LayoutKind.Explicit, Pack = 1)]
	// public struct Material {
	// 	
	// 	[FieldOffset(0)] public Vector4 Albedo;
	// 	[FieldOffset(16)] public Vector4 AmbientColor;
	// 	[FieldOffset(32)] public Vector4 DiffuseColor;
	// 	[FieldOffset(48)] public Vector4 SpecularColor;
	//
	// 	[FieldOffset(64)] public Texture2D DiffuseTexture;
	// 	[FieldOffset(72)] public Texture2D SpecularTexture;
	// 	[FieldOffset(80)] public Texture2D NormalTexture;
	// 	[FieldOffset(88)] public Texture2D HeightTexture;
	// 	
	// 	// [FieldOffset(64)] public Texture2D Texture;
	// 	// [FieldOffset(68)] public int TextureLayer;
	//
	// 	public Material() {
	// 		Albedo = new(1, 1, 1, 1);
	// 		Ambient = new(1, 1, 1, 1);
	// 		Diffuse = new(1, 1, 1, 1);
	// 		Specular = new(1, 1, 1, 1);
	// 		
	// 		Texture = Texture2D.DefaultTexture;
	// 		TextureLayer = -1;
	// 	}
	//
	// 	public void Load(ShaderProgram shader) {
	// 		shader.SetUniform("material.color", Albedo);
	// 		shader.SetUniform("material.tex_layer", TextureLayer);
	// 		
	// 		if(TextureLayer < 0) Texture.Bind();
	// 	}
	// }

	public class Material {

		public Vector4 Albedo = new(1, 1, 1, 1);
		public Vector4 AmbientColor = new(1, 1, 1, 1);
		public Vector4 DiffuseColor = new(1, 1, 1, 1);
		public Vector4 SpecularColor = new(1, 1, 1, 1);

		public List<(TextureType, Texture2D)> Textures { get; init; } = new() {
			(TextureType.Diffuse, Texture2D.DefaultTexture)
		};

		public void Load(ShaderProgram shader) {
			shader.SetUniform("material.albedo", Albedo);
			shader.SetUniform("material.ambient_color", AmbientColor);
			shader.SetUniform("material.diffuse_color", DiffuseColor);
			shader.SetUniform("material.specular_color", SpecularColor);

			int[] textureCounter = new int[(int) TextureType.Unknown];
			int textureUnit = 0;
			
			foreach(var (type, texture) in Textures) {
				int textureIndex = textureCounter[(int) type];

				if(textureIndex >= 4) {
					throw new ArgumentOutOfRangeException(
						nameof(textureIndex), "Only a maximum of 4 textures of one type are supported");
				}
				
				string uniformName = $"tex_{type.ToString().ToLower()}_{textureIndex}";
			
				shader.SetUniform(uniformName, textureUnit);
				texture.Bind(textureUnit);
				
				textureCounter[(int) type]++;
				textureUnit++;
			}
		}
		
		public enum TextureType {
			
			Diffuse,
			Specular,
			Normal,
			Height,
			Unknown
		}
	}
}