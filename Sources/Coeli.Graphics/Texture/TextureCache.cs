using System.Numerics;
using Serilog;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Texture {
	
	class TextureCache {

		private static readonly Dictionary<GL, TextureCache> LOCAL_CACHES = new();

		private readonly Dictionary<string, Texture<Vector2>> _textures = new();

		public static Texture<Vector2>? Get(GL gl, string name) {
			if(LOCAL_CACHES.ContainsKey(gl)) {
				Log.Verbose($"Getting texture for [{name}] from cache");
				return LOCAL_CACHES[gl]._textures[name];
			}

			return null;
		}

		public static void Set(GL? gl, string name, Texture<Vector2> texture) {
			Log.Verbose($"Caching texture for [{name}]");
			
			if(LOCAL_CACHES.ContainsKey(gl)) {
				LOCAL_CACHES[gl]._textures[name] = texture;
			}

			var cache = new TextureCache();
			cache._textures[name] = texture;

			LOCAL_CACHES[gl] = cache;
		}
	}

	unsafe class TextureDataCache {

		private static readonly Dictionary<string, TextureData> TEXTURES = new();

		public static TextureData? Get(string name) {
			if(TEXTURES.ContainsKey(name)) {
				Log.Verbose($"Getting texture data for [{name}] from cache");
				return TEXTURES[name];
			}
			
			return null;
		}

		public static void Set(string name, TextureData data) {
			Log.Verbose($"Caching texture data for [{name}]");
			TEXTURES[name] = data;
		}

		public readonly struct TextureData {

			public int Width { get; init; }
			public int Height { get; init; }
			public void*[] Data { get; init; }
		}
	}
}