using System.Numerics;
using Coelum.Configuration;
using Coelum.Graphics.OpenGL;
using Coelum.LanguageExtensions;
using Coelum.Resources;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coelum.Graphics.Texture {
	
	public class Texture2D : Texture<Vector2> {
		
		public static Texture2D DefaultTexture {
			get {
				var resource = Module.RESOURCES[Resource.Type.TEXTURE, "default"];
				
				if(Cache.GLOBAL.TryGet(resource, out var texture)) {
					return texture;
				}
				
				var data = resource.ReadBytes();

				if(data == null) {
					throw new Exception("Couldn't read the default texture. This should not happen :(");
				}
				
				texture = Create("default", data);
				Cache.GLOBAL.Set(resource, texture);
				return texture;
			}
		}
		
		public Texture2D(Vector2 size) : base(TextureTarget.Texture2D, size) { }
		public Texture2D(int width, int height) : base(TextureTarget.Texture2D, new(width, height)) { }

		[Obsolete("Not supported. Use Bind(ShaderProgram) instead", true)]
		public override void Bind() {
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(Target, Id);
		}

		public static Texture2D Load(Resource resource) {
			if(Cache.GLOBAL.TryGet(resource,
			                       out var texture)) {
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
				
				GLManager.SetDefaultsForTextureCreation(texture.Target, gl);
					
				image.ProcessPixelRows(accessor => {
					var data = new void*[image.Height];
					
					for(int y = 0; y < accessor.Height; y++) {
						fixed(void* addr = accessor.GetRowSpan(y)) {
							data[y] = addr;
						}
					}
					
					gl.TexImage2D(texture.Target, 0, InternalFormat.Rgba8,
					              (uint) image.Width, (uint) image.Height, 0,
					              PixelFormat.Rgba, PixelType.UnsignedByte, data[0]);
				});
			}
			
			if(EngineOptions.Texture.Mipmapping) gl.GenerateMipmap(texture.Target);
			return texture;
		}
		
		private class Cache : TextureCacheBase<Texture2D> {

			public static readonly Cache GLOBAL = new();
		}
		
		public static readonly IShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};

		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {

			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "texture2D";
			public string Path => "Overlays.Texture2D";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) {
				shader.SetUniform("tex2d_sampler", 1);
			}
		}
	}
}