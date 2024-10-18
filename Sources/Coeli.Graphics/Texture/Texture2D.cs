using System.Numerics;
using System.Runtime.Versioning;
using Coeli.Configuration;
using Coeli.Graphics.OpenGL;
using Coeli.LanguageExtensions;
using Coeli.Resources;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Shader = Coeli.Graphics.OpenGL.Shader;

namespace Coeli.Graphics.Texture {
	
	public class Texture2D : Texture<Vector2>, IShaderLoadable {
		
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

		public override void Bind() {
			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(Target, Id);
		}

		public void Load(ShaderProgram shader) {
			shader.EnableOverlays(OVERLAYS);
			Bind();
		}

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

			using(var image = Image.Load<Rgba32>(data)) {
				texture = new(gl, new(image.Width, image.Height));
				
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
		
		public static readonly IShaderOverlay[] OVERLAYS = [
			FragmentShaderOverlay.OVERLAY
		];

		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {

			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "texture2D";
			public string Path => "Overlays.Texture2D";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) {
				shader.SetUniform("tex2DSampler", 1);
			}
		}
	}
}