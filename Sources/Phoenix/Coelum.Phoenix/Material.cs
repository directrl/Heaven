using System.Numerics;
using System.Runtime.InteropServices;
using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.Texture;
using Coelum.Resources;
using Silk.NET.OpenGL;
using ShadingModel = Silk.NET.OpenGL.ShadingModel;

namespace Coelum.Phoenix {
	
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

			int textureUnit = 0;
			
			foreach(var (type, texture) in Textures) {
				string uniformName = $"material.tex_{type.ToString().ToLower()}";
			
				shader.SetUniform(uniformName, textureUnit);
				texture.Bind(textureUnit);

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

		public static readonly IShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};

		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "material";
			public string Path => "Overlays.Material";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
	}
}