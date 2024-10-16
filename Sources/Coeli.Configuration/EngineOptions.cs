using Silk.NET.OpenGL;

namespace Coeli.Configuration {
	
	public static class EngineOptions {

		public const String BASENAME = "heaven";
		
		public static void FromArgs(string[] args) { }

		public static class Texture {

			public static uint MinFilter { get; set; } = (int) TextureMinFilter.NearestMipmapLinear;
			public static uint MagFilter { get; set; } = (int) TextureMagFilter.Nearest;

			public static bool Mipmapping { get; set; } = true;
			public static uint MipmapLevel { get; set; } = 5;
		}
	}
}