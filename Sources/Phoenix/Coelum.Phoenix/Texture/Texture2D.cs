using System.Numerics;
using Coelum.Configuration;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coelum.Phoenix.Texture {
	
	public class Texture2D : Texture<Vector2> {
		
		public static Texture2D DefaultTexture {
			get => Load(Module.RESOURCES[ResourceType.TEXTURE, "default.png"]);
		}
		
		public Texture2D(Vector2 size) : base(TextureTarget.Texture2D, size) { }
		public Texture2D(int width, int height) : base(TextureTarget.Texture2D, new(width, height)) { }
		
		public static Texture2D Load(IResource resource) {
			if(Cache.GLOBAL.TryGet(resource, out var texture)) {
				return texture;
			}

			var data = resource.ReadBytes();
			if(data == null) return DefaultTexture;

			texture = Create(resource.UID, data);
			Cache.GLOBAL.Set(resource, texture);
			return texture;
		}
		
		private unsafe static Texture2D Create(string name, byte[] data) {
			Log.Debug($"Creating texture for [{name}]");

			Texture2D texture;

			using(var image = Image.Load<Rgba32>(data)) {
				texture = new(new(image.Width, image.Height));
				texture.Bind();
				
				GLManager.SetDefaultsForTextureCreation(texture.Target);
				
				image.ProcessPixelRows(accessor => {
					var data = new void*[image.Height];
					
					for(int y = 0; y < accessor.Height; y++) {
						fixed(void* addr = accessor.GetRowSpan(y)) {
							data[y] = addr;
						}
					}
					
					Gl.TexImage2D(texture.Target, 0, InternalFormat.Rgba8,
					              (uint) image.Width, (uint) image.Height, 0,
					              PixelFormat.Rgba, PixelType.UnsignedByte, data[0]);
				});
			}
			
			if(EngineOptions.Texture.Mipmapping) Gl.GenerateMipmap(texture.Target);
			texture.Name = name;
			return texture;
		}
		
		private class Cache : TextureCacheBase<Texture2D> {

			public static readonly Cache GLOBAL = new();
		}
	}
}