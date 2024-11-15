using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.LanguageExtensions.Serialization;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.Texture;
using Coelum.Resources;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using Shader = Coelum.Phoenix.OpenGL.Shader;
using ShadingModel = Silk.NET.OpenGL.ShadingModel;

namespace Coelum.Phoenix {
	
	public struct Material : ISerializable<Material> {
		
		public enum TextureType {
			
			Diffuse,
			Specular,
			Normal,
			Height,
			Unknown
		}
		
		private static readonly Dictionary<TextureType, string> _UNIFORM_NAMES = new() {
			{ TextureType.Diffuse, "material.tex_diffuse" },
			{ TextureType.Specular, "material.tex_specular" },
			{ TextureType.Normal, "material.tex_normal" },
			{ TextureType.Height, "material.tex_height" },
		};

		public Vector4 Albedo = new(1, 1, 1, 1);
		public Vector4 AmbientColor = new(1, 1, 1, 1);
		public Vector4 DiffuseColor = new(1, 1, 1, 1);
		public Vector4 SpecularColor = new(1, 1, 1, 1);

		public float Shininess = 0.3f;
		public float Reflectivity = 0.5f;

		public List<(TextureType Type, Texture2D Texture)> Textures { get; init; } = new() {
			(TextureType.Diffuse, Texture2D.DefaultTexture)
		};
		
		public Material() { }

		public void Load(ShaderProgram shader) {
			shader.SetUniform("material.albedo", Albedo);
			shader.SetUniform("material.ambient_color", AmbientColor);
			shader.SetUniform("material.diffuse_color", DiffuseColor);
			shader.SetUniform("material.specular_color", SpecularColor);

			shader.SetUniform("material.shininess", Shininess);
			shader.SetUniform("material.reflectivity", Reflectivity);
			
			int textureUnit = 0;
			
			foreach(var (type, texture) in Textures) {
				if(type == TextureType.Unknown) continue;
			
				shader.SetUniform(_UNIFORM_NAMES[type], 0);
				texture.Bind(textureUnit);

				textureUnit++;
			}
		}

	#region Serialization
		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject();
			{
				Albedo.Serializer().Serialize("albedo", writer);
				AmbientColor.Serializer().Serialize("ambient_color", writer);
				DiffuseColor.Serializer().Serialize("diffuse_color", writer);
				SpecularColor.Serializer().Serialize("specular_color", writer);
				
				writer.WriteNumber("shininess", Shininess);
				writer.WriteNumber("reflectivity", Reflectivity);
				
				// textures
				writer.WriteStartArray("textures");
				foreach(var texture in Textures) {
					writer.WriteStartObject();
					{
						writer.WriteNumber("type", (int) texture.Type);
						// TODO
						writer.WriteString("name", texture.Texture.Name);
					}
					writer.WriteEndObject();
				}
				writer.WriteEndArray();
			}
			writer.WriteEndObject();
		}
		
		public Material Deserialize(JsonNode node) {
			var albedo = new Vector4Serializer().Deserialize(node["albedo"]);
			var ambientColor = new Vector4Serializer().Deserialize(node["ambient_color"]);
			var diffuseColor = new Vector4Serializer().Deserialize(node["diffuse_color"]);
			var specularColor = new Vector4Serializer().Deserialize(node["specular_color"]);

			var shininess = node["shininess"].GetValue<float>();
			var reflectivity = node["reflectivity"].GetValue<float>();
			
			// textures
			var texturesJson = node["textures"].AsArray();
			var textures = new List<(TextureType Type, Texture2D Texture)>();

			foreach(var textureJson in texturesJson) {
				var type = (TextureType) textureJson["type"].GetValue<int>();
				var name = textureJson["name"].GetValue<string>();
				
				// TODO
			}

			return new() {
				Albedo = albedo,
				AmbientColor = ambientColor,
				DiffuseColor = diffuseColor,
				SpecularColor = specularColor,
				Shininess = shininess,
				Reflectivity = reflectivity,
				Textures = textures
			};
		}
	#endregion

	#region Overlays
		public static readonly ShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY,
			VertexShaderOverlay.OVERLAY
		};

		public class VertexShaderOverlay : ShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public override string Name => "material";
			public override string Path => "Overlays.Material";
			
			public override bool HasCall => false;
			
			public override ShaderType Type => ShaderType.VertexShader;
			public override ShaderPass Pass => ShaderPass.POSITION_PRE;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
		
		public class FragmentShaderOverlay : ShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public override string Name => "material";
			public override string Path => "Overlays.Material";
			
			public override ShaderType Type => ShaderType.FragmentShader;
			public override ShaderPass Pass => ShaderPass.COLOR_PRE;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
	#endregion
	}
}