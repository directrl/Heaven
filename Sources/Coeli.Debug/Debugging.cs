namespace Coeli.Debug {
	
	public static class Debugging {
		
		public static bool Enabled { get; set; }
		public static bool Verbose { get; set; }
		
		public static bool DumpShaders { get; set; }
		public static bool IgnoreMissingShaderUniforms { get; set; }
		
	#if DEBUG
		public static bool CheatEnabled { get; private set; }
	#else
		// hopefully the compiler inlines this property
		public static bool CheatEnabled => false;
	#endif

		public static void FromArgs(string[] args) {
			if(args.Contains("--debug")) {
				Enabled = true;
				
			#if DEBUG
				CheatEnabled = true;
			#endif
			}

			if(args.Contains("--verbose")) Verbose = true;
			if(args.Contains("--dump-shaders")) DumpShaders = true;
		}
	}
}