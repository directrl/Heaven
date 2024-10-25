using System.Numerics;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coelum.Phoenix.Texture {

	public class TextureArray : Texture<Vector2> {

		public int Layers { get; }
		public int LayerIndex { get; private set; }

		public TextureArray(Vector2 size, int layers)
			: base(TextureTarget.Texture2DArray, size) {

			Layers = layers;
			
			GLManager.SetDefaultsForTextureCreation(Target);
			
			Gl.TexStorage3D(
				Target,
				EngineOptions.Texture.TexStorage3DLevels,
				SizedInternalFormat.Rgba8,
				(uint) size.X, (uint) size.Y, (uint) Layers
			);
		}

		public void Add(Resource resource) {
			Bind();
			
			if(!LoadTexture(this, resource)) {
				throw new ArgumentException("Could not load resource", nameof(resource));
			}
		}

		public static TextureArray Create(params IResource[] resources) {
			int width = 0, height = 0;
			if(!LoadImage(resources[0],
			              image => {
				              width = image.Width;
				              height = image.Height;
			              })) {
				throw new ArgumentException("Could not load resources[0]", nameof(resources));
			}

			var texture = new TextureArray(new(width, height), resources.Length);
			
			for(int i = 0; i < resources.Length; i++) {
				if(!LoadTexture(texture, resources[i])) {
					throw new ArgumentException($"Could not load resources[{i}]", nameof(resources));
				}
			}
			
			return texture;
		}

		private static bool LoadImage(IResource resource, Action<Image<Rgba32>> callback) {
			resource.Cache = false; // textures should not be cached as they're uploaded to the GPU
			
			var data = resource.ReadBytes();
			if(data == null) return false;

			using(var image = Image.Load<Rgba32>(data)) {
				callback.Invoke(image);
			}

			return true;
		}
		
		private unsafe static bool LoadTexture(TextureArray texture, IResource resource) {
			return LoadImage(resource, image => {
				texture.Bind();
				
				image.ProcessPixelRows(accessor => {
					Tests.Assert(image.Width == (int) texture.Size.X, "All textures must have the same size");
					Tests.Assert(image.Height == (int) texture.Size.Y, "All textures must have the same size");
					
					var data = new void*[image.Height];
					
					for(int y = 0; y < accessor.Height; y++) {
						fixed(void* addr = accessor.GetRowSpan(y)) {
							data[y] = addr;
						}
					}
					
					Gl.TexSubImage3D(
						texture.Target,
						0,
						0, 0, texture.LayerIndex,
						(uint) image.Width, (uint) image.Height, 1,
						PixelFormat.Rgba, PixelType.UnsignedByte,
						data[0]
					);
				});

				if(EngineOptions.Texture.Mipmapping) Gl.GenerateMipmap(texture.Target);
				texture.LayerIndex++;
			});
		}

		public static readonly IShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};
		
		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {

			//public static readonly FragmentShaderOverlay GLOBAL = new();
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "textureArray";
			public string Path => "Overlays.TextureArray";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) {
				shader.SetUniform("texArray_sampler", 2);
			}
		}
	}
}