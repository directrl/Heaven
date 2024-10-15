using System.Numerics;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coeli.Graphics.Texture {
	
	public static class TextureManager {

		public static Texture<Vector2> DefaultTexture {
			get {
				var defaultTexture = TextureCache.Get(GLManager.Current, "default");
				if(defaultTexture != null) return defaultTexture;
				
				throw new Exception("Couldn't find the default texture. This should not happen :(");
			}
		}

		private unsafe static Texture<Vector2> Create(GL gl, string name, byte[] data) {
			Log.Debug($"Creating texture for [{name}]");
			
			Texture<Vector2> texture;

			//var textureData = TextureDataCache.Get(name);
			TextureDataCache.TextureData? textureData = null;

			if(textureData == null) {
				using(var image = Image.Load<Rgba32>(data)) {
					var rawData = new void*[image.Height];

					texture = new(gl, TextureTarget.Texture2D,
						new(image.Width, image.Height));
					
					gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8,
						(uint) image.Width, (uint) image.Height, 0,
						PixelFormat.Rgba, PixelType.UnsignedByte, null);
					
					image.ProcessPixelRows(accessor => {
						for(int y = 0; y < accessor.Height; y++) {
							fixed(void* addr = accessor.GetRowSpan(y)) {
								rawData[y] = addr;
								
								gl.TexSubImage2D(TextureTarget.Texture2D, 0,
									0, y,
									(uint) accessor.Width, 1,
									PixelFormat.Rgba, PixelType.UnsignedByte, addr);
							}
						}
					});
					
					TextureDataCache.Set(name, new() {
						Width = image.Width,
						Height = image.Height,
						Data = rawData
					});
				}
			} else {
				var info = textureData.Value;
				var rawData = info.Data;
				
				texture = new(gl, TextureTarget.Texture2D,
					new(info.Width, info.Height));
					
				gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8,
					(uint) info.Width, (uint) info.Height, 0,
					PixelFormat.Rgba, PixelType.UnsignedByte, null);

				for(int y = 0; y < info.Height; y++) {
					gl.TexSubImage2D(TextureTarget.Texture2D, 0,
						0, y,
						(uint) info.Width, 1,
						PixelFormat.Rgba, PixelType.UnsignedByte, rawData[y]);
				}
			}
			
			GLManager.SetDefaultsForTextureCreation(gl);
			return texture;
		}

		public static Texture<Vector2> Load(string name, byte[] data, GL? gl = null) {
			if(gl == null) gl = GLManager.Current;

			var cachedTexture = TextureCache.Get(gl, name);
			if(cachedTexture != null) return cachedTexture;
			
			var texture = Create(gl, name, data);
			TextureCache.Set(gl, name, texture);
			return texture;
		}

		public static Texture<Vector2> Load(Resource resource, GL? gl = null) {
			if(gl == null) gl = GLManager.Current;

			var cachedTexture = TextureCache.Get(gl, resource.UID);
			if(cachedTexture != null) return cachedTexture;

			var data = resource.ReadBytes();
			if(data == null) return DefaultTexture;
			
			var texture = Create(gl, resource.UID, data);
			TextureCache.Set(gl, resource.UID, texture);
			return texture;
		}
	}
}