using System.Diagnostics;
using Coelum.Debug;
using Coelum.Phoenix.Texture;
using Coelum.Resources;
using Serilog;
using Silk.NET.Assimp;
using AiScene = Silk.NET.Assimp.Scene;
using AiNode = Silk.NET.Assimp.Node;
using AiMesh = Silk.NET.Assimp.Mesh;
using AiFace = Silk.NET.Assimp.Face;
using AiMaterial = Silk.NET.Assimp.Material;
using AiTextureType = Silk.NET.Assimp.TextureType;
using Material = Coelum.Phoenix.Material;
using Mesh = Coelum.Phoenix.Mesh;
using PrimitiveType = Silk.NET.OpenGL.PrimitiveType;

namespace Coelum.Phoenix.ModelLoading {
	
	public static class ModelLoader {
		
		public static readonly Model DEFAULT_CUBE = new("__default_cube__") {
			Meshes = new() {
				new(PrimitiveType.Triangles,
				    new Vertex[] {
					    new(new(-0.5f, 0.5f, 0.5f)),
					    new(new(-0.5f, -0.5f, 0.5f)),
					    new(new(0.5f, -0.5f, 0.5f)),
					    new(new(0.5f, 0.5f, 0.5f)),
					    new(new(-0.5f, 0.5f, -0.5f)),
					    new(new(0.5f, 0.5f, -0.5f)),
					    new(new(-0.5f, -0.5f, -0.5f)),
					    new(new(0.5f, -0.5f, -0.5f)),
				    },
				    new uint[] {
					    // Front face
					    0, 1, 3, 3, 1, 2,
					    // Top Face
					    4, 0, 3, 5, 4, 3,
					    // Right face
					    3, 2, 7, 5, 3, 7,
					    // Left face
					    6, 1, 0, 6, 0, 4,
					    // Bottom face
					    2, 1, 6, 2, 6, 7,
					    // Back face
					    7, 6, 4, 7, 4, 5,
				    })
				{
					MaterialIndex = 0
				}
			}
		};

		static ModelLoader() {
			_ = new GlobalAssimp(Assimp.GetApi());
		}

		public static Model? Load(IResource resource) {
			if(ModelRegistry.TryGet(resource, out var model)) {
				return model;
			}

			if(!resource.Name.EndsWith(".gltf") && !resource.Name.EndsWith(".glb")) {
				Log.Warning("[MODEL LOADER] Loading non-glTF models is experimental and might not work correctly");
			}

			if(resource.Name.EndsWith(".gltf") && resource is not ExternalResource) {
				throw new ArgumentException(".gltf models can only be loaded as an external resource", nameof(resource));
			}

			if(resource is ExternalResource eResource) {
				model = Create(eResource.FullPath, path: eResource.FullPath);
			} else {
				var data = resource.ReadBytes();
				if(data == null) return null;

				model = Create(resource.UID, data: data);
			}
			
			return model;
		}

		private unsafe static Model Create(string name, byte[]? data = null, string? path = null, uint flags
			                                   = (uint) (PostProcessSteps.GenerateSmoothNormals 
				                                   | PostProcessSteps.JoinIdenticalVertices 
				                                   | PostProcessSteps.Triangulate 
				                                   | PostProcessSteps.FixInFacingNormals
				                                   | PostProcessSteps.CalculateTangentSpace 
				                                   | PostProcessSteps.LimitBoneWeights 
				                                   | PostProcessSteps.PreTransformVertices 
				                                   | PostProcessSteps.OptimizeMeshes
				                                   | PostProcessSteps.FlipUVs)) {
			
			Tests.Assert(data != null || path != null);
			Log.Debug($"[MODEL LOADER] Creating model for [{name}]");

			var scene = path != null
				? Ai.ImportFile(path, flags)
				: Ai.ImportFileFromMemory(data, (uint) data.Length, flags, (byte*) null);
			
			if(scene == null || scene->MFlags == (uint) SceneFlags.Incomplete || scene->MRootNode == null) {
				throw new LoadingException(Ai.GetErrorStringS());
			}
			
			Log.Verbose("[MODEL LOADER] BEGIN PROCESSING");
			var sw = Stopwatch.StartNew();

			var model = new Model(name);
			model.Materials.Clear(); // we don't want any default materials
			
			ProcessScene(ref model, scene);
			ProcessNode(ref model, scene, scene->MRootNode);
			
			Ai.FreeScene(scene);
			
			sw.Stop();
			Log.Verbose($"[MODEL LOADER] DONE IN {sw.ElapsedMilliseconds}ms");
			return model;
		}

		private unsafe static void ProcessScene(ref Model model, AiScene* aiScene) {
			for(int i = 0; i < aiScene->MNumMaterials; i++) {
				var material = new Material();
				material.Textures.Clear(); // we don't want any default textures
			
				ProcessMaterial(ref model, ref material,
				                aiScene, aiScene->MMaterials[i]);
				
				model.Materials.Add(material);
			}
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
			var vertices = new Vertex[aiMesh->MNumVertices];
			var indices = new List<uint>();

		#region Vertices
			for(int i = 0; i < aiMesh->MNumVertices; i++) {
				var vertex = new Vertex() {
					Position = aiMesh->MVertices[i]
				};

				if(aiMesh->MNormals != null) {
					vertex.Normal = aiMesh->MNormals[i];
				}

				if(aiMesh->MTextureCoords[0] != null) {
					var texCoords = aiMesh->MTextureCoords[0][i];
					vertex.TexCoords = new(texCoords.X, texCoords.Y);
				}

				vertices[i] = vertex;
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
			
			var mesh = new Mesh(
				PrimitiveType.Triangles,
				vertices,
				indices.ToArray()
			) {
				MaterialIndex = (int) aiMesh->MMaterialIndex
			};

			model.Meshes.Add(mesh);
		}

		private unsafe static void ProcessMaterial(ref Model model, ref Material material,
		                                           AiScene* aiScene, AiMaterial* aiMaterial) {
			if(aiMaterial == null) return;

			Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorAmbient,
			                    0, 0, ref material.AmbientColor);
			Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorDiffuse,
			                    0, 0, ref material.DiffuseColor);
			Ai.GetMaterialColor(aiMaterial, Assimp.MatkeyColorSpecular,
			                    0, 0, ref material.SpecularColor);

			uint max = 1; // TODO what are you supposed to do with this?
			Ai.GetMaterialFloatArray(aiMaterial, Assimp.MaterialShininess,
			                         0, 0, ref material.Shininess, ref max);
			Ai.GetMaterialFloatArray(aiMaterial, Assimp.MaterialReflectivity,
			                         0, 0, ref material.Reflectivity, ref max);
			
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
				Log.Warning($"[MODEL LOADER: {model.Name}] Material has no textures");
				
				//material.Textures.Add((Material.TextureType.Diffuse, Texture2D.DefaultTexture));
				return;
			}
			
			for(uint i = 0; i < texCount; i++) {
				AssimpString path;
				Ai.GetMaterialTexture(aiMaterial, textureType, i, &path,
				                      null, null, null, null, null, null);

				Texture2D? texture = null;
				
				if(aiScene->MNumTextures > 0 && path.Data[0] == '*') {
					int textureId = int.Parse((string) path.AsString.Replace("*", ""));
					var aiTexture = aiScene->MTextures[textureId];

					if(aiTexture->MHeight == 0) {
						var data = new Span<byte>(aiTexture->PcData, (int) aiTexture->MWidth);
						var resource = new RawResource(ResourceType.TEXTURE, $"{model.Name}#{textureId}", data);
						texture = Texture2D.Load(resource);
					}
				} else {
					Log.Debug($"[MODEL LOADER: {model.Name}] Material texture path: {path.AsString}");

					string? dirPath = Path.GetDirectoryName(model.Name);
					if(dirPath == null) throw new ArgumentException(nameof(model.Name));

					string filePath = Path.Combine(dirPath, Uri.UnescapeDataString(path.AsString));
					
					var textureResource = new ExternalResourceManager()[ResourceType.TEXTURE, filePath];
					texture = Texture2D.Load(textureResource);
				}

				if(texture == null) {
					Log.Warning($"[MODEL LOADER: {model.Name}] Could not load texture for material");
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
					Log.Warning($"[MODEL LOADER: {model.Name}] Unknown material texture type: {textureType}");
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