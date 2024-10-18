using System.Numerics;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Graphics.OpenGL;
using Coelum.LanguageExtensions;
using Coelum.Resources;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coelum.Graphics.Texture {

	public class TextureArray : Texture<Vector2> {

		public int Layers { get; }
		public int LayerIndex { get; private set; }

		public TextureArray(GL gl, Vector2 size, int layers)
			: base(gl, TextureTarget.Texture2DArray, size) {

			Layers = layers;
			
			GLManager.SetDefaultsForTextureCreation(Target, GL);
			
			gl.TexStorage3D(
				Target,
				EngineOptions.Texture.TexStorage3DLevels,
				SizedInternalFormat.Rgba8,
				(uint) size.X, (uint) size.Y, (uint) Layers
			);
		}

		public void Add(Resource resource) {
			Bind();
			
			if(!LoadTexture(this, resource, GL)) {
				throw new ArgumentException("Could not load resource", nameof(resource));
			}
		}

		[Obsolete("Not supported. Use Bind(ShaderProgram) instead", true)]
		public override void Bind() {
			GL.ActiveTexture(TextureUnit.Texture2);
			GL.BindTexture(Target, Id);
		}

		public static TextureArray Create(GL? gl, params Resource[] resources) {
			gl ??= GLManager.Current;

			int width = 0, height = 0;
			if(!LoadImage(resources[0],
			              image => {
				              width = image.Width;
				              height = image.Height;
			              })) {
				throw new ArgumentException("Could not load resources[0]", nameof(resources));
			}

			var texture = new TextureArray(gl, new(width, height), resources.Length);
			
			for(int i = 0; i < resources.Length; i++) {
				if(!LoadTexture(texture, resources[i], gl)) {
					throw new ArgumentException($"Could not load resources[{i}]", nameof(resources));
				}
			}
			
			return texture;
		}

		private static bool LoadImage(Resource resource, Action<Image<Rgba32>> callback) {
			resource.Cache = false; // textures should not be cached as they're uploaded to the GPU
			
			var data = resource.ReadBytes();
			if(data == null) return false;

			using(var image = Image.Load<Rgba32>(data)) {
				callback.Invoke(image);
			}

			return true;
		}
		
		private unsafe static bool LoadTexture(TextureArray texture, Resource resource, GL gl) {
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
					
					gl.TexSubImage3D(
						texture.Target,
						0,
						0, 0, texture.LayerIndex,
						(uint) image.Width, (uint) image.Height, 1,
						PixelFormat.Rgba, PixelType.UnsignedByte,
						data[0]
					);
				});

				if(EngineOptions.Texture.Mipmapping) gl.GenerateMipmap(texture.Target);
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