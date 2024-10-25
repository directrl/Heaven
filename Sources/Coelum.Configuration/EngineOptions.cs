namespace Coelum.Configuration {
	
	public static class EngineOptions {

		public const String BASENAME = "heaven";

		public static bool VerticalSync { get; set; } = true;

		public static void FromArgs(string[] args) {
			if(args.Contains("--no-vsync")) VerticalSync = false;
		}

		public static class Texture {

			public static uint MinFilter { get; set; } = 9986; // GL_NEAREST_MIPMAP_LINEAR
			public static uint MagFilter { get; set; } = 9728; // GL_NEAREST

			public static bool Mipmapping { get; set; } = true;
			
			public static uint TexStorage3DLevels { get; set; } = 1;
		}
	}
}