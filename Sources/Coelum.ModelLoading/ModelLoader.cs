using System.Diagnostics;
using System.Dynamic;
using System.Numerics;
using Coelum.Graphics;
using Coelum.Graphics.Texture;
using Coelum.LanguageExtensions;
using Coelum.Resources;
using Serilog;
using Silk.NET.Assimp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using AiScene = Silk.NET.Assimp.Scene;
using AiNode = Silk.NET.Assimp.Node;
using AiMesh = Silk.NET.Assimp.Mesh;
using AiFace = Silk.NET.Assimp.Face;
using AiMaterial = Silk.NET.Assimp.Material;
using AiTextureType = Silk.NET.Assimp.TextureType;
using Material = Coelum.Graphics.Material;
using Mesh = Coelum.Graphics.Mesh;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace Coelum.ModelLoading {
	
	public static class ModelLoader {

		static ModelLoader() {
			_ = new GlobalAssimp(Assimp.GetApi());
		}

		public static Model? Load(IResource resource) {
			if(ModelCache.GLOBAL.TryGet(resource, out var model)) {
				return model;
			}

			var data = resource.ReadBytes();
			if(data == null) return null;

			model = Create(resource.UID, data);
			ModelCache.GLOBAL.Set(resource, model);
			return model;
		}

		private unsafe static Model Create(string name, byte[] data, uint flags
			                                   = (uint) (PostProcessSteps.GenerateSmoothNormals 
				                                   | PostProcessSteps.JoinIdenticalVertices 
				                                   | PostProcessSteps.Triangulate 
				                                   | PostProcessSteps.FixInFacingNormals
				                                   | PostProcessSteps.CalculateTangentSpace 
				                                   | PostProcessSteps.LimitBoneWeights 
				                                   | PostProcessSteps.PreTransformVertices 
				                                   | PostProcessSteps.OptimizeMeshes
				                                   | PostProcessSteps.FlipUVs)) {
			
			Log.Debug($"[MODEL LOADER] Creating model for [{name}]");

			var scene = Ai.ImportFileFromMemory(data, (uint) data.Length, flags, (byte*) null);
			if(scene == null || scene->MFlags == (uint) SceneFlags.Incomplete || scene->MRootNode == null) {
				throw new LoadingException(Ai.GetErrorStringS());
			}
			
			Log.Verbose("[MODEL LOADER] BEGIN PROCESSING");
			var sw = Stopwatch.StartNew();
			
			var model = new Model(name, new List<Mesh>());
			ProcessNode(ref model, scene, scene->MRootNode);
			
			sw.Stop();
			Log.Verbose($"[MODEL LOADER] DONE IN {sw.ElapsedMilliseconds}ms");
			return model;
		}

		private unsafe static void ProcessNode(ref Model model, AiScene* aiScene, AiNode* aiNode) {
			for(int i = 0; i < aiNode->MNumMeshes; i++) {
				ProcessMesh(ref model, aiScene, aiScene->MMeshes[i]);
			}

			for(int i = 0; i < aiNode->MNumChildren; i++) {
				ProcessNode(ref model, aiScene, aiNode->MChildren[i]);
			}
		}

		private unsafe static void ProcessMesh(ref Model model, AiScene* aiScene, AiMesh* aiMesh) {
			var positions = new List<float>();
			var indices = new List<uint>();
			var texCoords = new List<float>();
			var normals = new List<float>();

		#region Vertices
			for(int i = 0; i < aiMesh->MNumVertices; i++) {
				var vertex = aiMesh->MVertices[i];
				positions.Add(vertex.X);
				positions.Add(vertex.Y);
				positions.Add(vertex.Z);

				if(aiMesh->MNormals != null) {
					var normal = aiMesh->MNormals[i];
					normals.Add(normal.X);
					normals.Add(normal.Y);
					normals.Add(normal.Z);
				}

				if(aiMesh->MTextureCoords[0] != null) {
					var texCoord = aiMesh->MTextureCoords[0][i];
					texCoords.Add(texCoord.X);
					texCoords.Add(texCoord.Y);
					//texCoords.Add(texCoord.Z);
				}
			}
		#endregion

		#region Indices
			for(int i = 0; i < aiMesh->MNumFaces; i++) {
				var face = aiMesh->MFaces[i];

				for(int j = 0; j < face.MNumIndices; j++) {
					indices.Add(face.MIndices[j]);
				}
			}
		#endregion

			var material = new Material() {
				Textures = new()
			};
			
			ProcessMaterial(ref model, ref material,
			                aiScene, aiScene->MMaterials[aiMesh->MMaterialIndex]);
			
			var mesh = new Mesh(
				PrimitiveType.Triangles,
				positions.ToArrayNoCopy(),
				indices.ToArrayNoCopy(),
				texCoords.ToArrayNoCopy(),
				normals.ToArrayNoCopy()
			) {
				Material = material
			};

			model.Meshes.Add(mesh);
		}

		private unsafe static void ProcessMaterial(ref Model model, ref Material material,
		                                           AiScene* aiScene, AiMaterial* aiMaterial) {
			if(aiMaterial == null) return;
			
			var ambient = new Vector4();
			if(Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorAmbient,
			                       0, 0, ref ambient) == Return.Success) {
					
				material.AmbientColor = ambient with {
					W = 1
				};
			} else {
				Log.Warning($"Could not get the ambient color for material");
			}
				
			var diffuse = new Vector4();
			if(Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorDiffuse,
			                       0, 0, ref diffuse) == Return.Success) {
					
				material.DiffuseColor = diffuse with {
					W = 1
				};
			} else {
				Log.Warning($"Could not get the diffuse color for material");
			}
				
			var specular = new Vector4();
			if(Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorSpecular,
			                       0, 0, ref specular) == Return.Success) {
					
				material.SpecularColor = specular with {
					W = 1
				};
			} else {
				Log.Warning($"Could not get the specular color for material");
			}
			
			ProcessMaterialTextures(ref model, ref material, aiScene, aiMaterial, AiTextureType.Diffuse);
			ProcessMaterialTextures(ref model, ref material, aiScene, aiMaterial, AiTextureType.Specular);
			ProcessMaterialTextures(ref model, ref material, aiScene, aiMaterial, AiTextureType.Normals);
			ProcessMaterialTextures(ref model, ref material, aiScene, aiMaterial, AiTextureType.Height);
		}

		private unsafe static void ProcessMaterialTextures(ref Model model, ref Material material,
		                                                   AiScene* aiScene, 
		                                                   AiMaterial* aiMaterial,
		                                                   AiTextureType textureType) {

			uint texCount = Ai.GetMaterialTextureCount(aiMaterial, textureType);

			if(texCount == 0) {
				Log.Warning($"[MODEL LOADER ({model.Name})] No textures found for material; is the model exported correctly?");
			}
			
			for(uint i = 0; i < texCount; i++) {
				AssimpString path;
				Ai.GetMaterialTexture(aiMaterial, textureType, i, &path,
				                      null, null, null, null, null, null);

				Texture2D? texture = null;
				
				if(aiScene->MNumTextures > 0 && path.Data[0] == '*') {
					int textureId = int.Parse(path.AsString.Replace("*", ""));
					var aiTexture = aiScene->MTextures[textureId];

					if(aiTexture->MHeight == 0) {
						var data = new Span<byte>(aiTexture->PcData, (int) aiTexture->MWidth);
						var resource = new RawResource(ResourceType.TEXTURE, $"{model.Name}#{textureId}", data);
						texture = Texture2D.Load(resource);
					}
				} else {
					Log.Debug($"[MODEL LOADER] Material texture path: {path.AsString}");
					// TODO
				}

				if(texture == null) {
					Log.Warning($"[MODEL LOADER] Could not load texture for model [{model.Name}]");
					continue;
				}

				var trueType = textureType switch {
					AiTextureType.Diffuse => Material.TextureType.Diffuse,
					AiTextureType.Specular => Material.TextureType.Specular,
					AiTextureType.Normals => Material.TextureType.Normal,
					AiTextureType.Height => Material.TextureType.Height,
					_ => Material.TextureType.Unknown
				};

				if(trueType == Material.TextureType.Unknown) {
					Log.Warning($"[MODEL LOADER] Unknown material texture type: {textureType}");
					continue;
				}
				
				material.Textures.Add((trueType, texture));
			}
		}

		public class LoadingException : Exception {
			
			public LoadingException(string message) : base(message) { }
		}
	}
}