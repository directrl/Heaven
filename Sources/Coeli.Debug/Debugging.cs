namespace Coeli.Debug {
	
	public static class Debugging {
		
		public static bool Enabled { get; set; }
		public static bool Verbose { get; set; }
		
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
		}
	}
}