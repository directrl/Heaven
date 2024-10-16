using System.Numerics;
using Coeli.Configuration;
using Coeli.Graphics.OpenGL;
using Coeli.LanguageExtensions;
using Coeli.Resources;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coeli.Graphics.Texture {
	
	public class Texture2D : Texture<Vector2> {
		
		public static Texture2D DefaultTexture {
			get {
				if(Cache.GLOBAL.TryGet(
					   new Cache.Entry {
						   GL = GLManager.Current,
						   UID = "default"
					   },
					   out var texture)) {
					return texture;
				}
				
				var resource = Module.RESOURCES[Resource.Type.TEXTURE, "default"];
				var data = resource.ReadBytes();

				if(data == null) {
					throw new Exception("Couldn't find the default texture. This should not happen :(");
				}
				
				texture = Create(
					"default",
					data,
					GLManager.Current
				);
				
				Cache.GLOBAL.Set(new Cache.Entry { GL = GLManager.Current, UID = "default" },
					texture);
				return texture;
			}
		}
		
		public Texture2D(GL gl, Vector2 size) : base(gl, TextureTarget.Texture2D, size) { }

		public static Texture2D Load(Resource resource, GL? gl = null) {
			gl ??= GLManager.Current;
			
			if(Cache.GLOBAL.TryGet(new Cache.Entry { GL = gl, UID = resource.UID },
			                       out var texture)) {
				return texture;
			}

			var data = resource.ReadBytes();
			if(data == null) return DefaultTexture;
			return Create(resource.UID, data, gl);
		}
		
		private unsafe static Texture2D Create(string name, byte[] data, GL? gl = null) {
			gl ??= GLManager.Current;
			Log.Debug($"Creating texture for [{name}]");

			Texture2D texture;
			
			GLManager.SetDefaultsForTextureCreation(gl);

			using(var image = Image.Load<Rgba32>(data)) {
				texture = new(gl, new(image.Width, image.Height));
					
				gl.TexImage2D(texture.Target, 0, InternalFormat.Rgba8,
					(uint) image.Width, (uint) image.Height, 0,
					PixelFormat.Rgba, PixelType.UnsignedByte, null);
					
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
			}
			
			if(EngineOptions.Texture.Mipmapping) gl.GenerateMipmap(texture.Target);
			return texture;
		}
		
		class Cache : TextureCacheBase<Texture2D> {

			public static readonly Cache GLOBAL = new();
		}
	}
}