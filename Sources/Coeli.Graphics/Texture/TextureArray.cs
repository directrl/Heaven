using System.Numerics;
using Coeli.Configuration;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coeli.Graphics.Texture {

	public class TextureArray : Texture<Vector2> {

		private readonly List<Texture2D> _textures;
		private bool _mipmapsReady;

		public int Layers { get; }
		public int LayerIndex { get; private set; }

		public TextureArray(GL gl, Vector2 size, int layers)
			: base(gl, TextureTarget.Texture2DArray, size) {

			Layers = layers;
			
			gl.TexStorage3D(
				Target,
				EngineOptions.Texture.MipmapLevel,
				SizedInternalFormat.Rgba8,
				(uint) size.X, (uint) size.Y, (uint) Layers
			);
			
			GLManager.SetDefaultsForTextureCreation(gl);
		}

		public void Add(Resource resource) {
			Bind();
			
			if(!LoadTexture(this, resource, GL)) {
				throw new ArgumentException("Could not load resource", nameof(resource));
			}
		}

		public override void Bind() {
			base.Bind();
			
			if(!_mipmapsReady && EngineOptions.Texture.Mipmapping) {
				GLManager.SetDefaultsForTextureCreation(GL);
				GL.GenerateMipmap(Target);

				_mipmapsReady = true;
			}
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
			var data = resource.ReadBytes();
			if(data == null) return false;

			using(var image = Image.Load<Rgba32>(data)) {
				callback.Invoke(image);
			}

			return true;
		}
		
		private unsafe static bool LoadTexture(TextureArray texture, Resource resource, GL gl) {
			return LoadImage(resource, image => {
				gl.TexSubImage3D(
					texture.Target,
					0,
					0, 0, texture.LayerIndex,
					(uint) image.Width, (uint) image.Height, 1,
					PixelFormat.Rgb, PixelType.UnsignedByte,
					null
				);
					
				image.ProcessPixelRows(accessor => {
					for(int y = 0; y < accessor.Height; y++) {
						fixed(void* addr = accessor.GetRowSpan(y)) {
							gl.TexSubImage2D(
								texture.Target,
								0,
								0, y,
								(uint) accessor.Width, 1,
								PixelFormat.Rgba, PixelType.UnsignedByte,
								addr
							);
						}
					}
				});

				texture.LayerIndex++;
			});
		}
	}
}