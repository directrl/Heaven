namespace Coeli.Debug {
	
	public class ExperimentalFlags {

		public static bool ForceSDL { get; set; }
		public static bool ForceWayland { get; set; }

		public static void FromArgs(string[] args) {
			if(args.Contains("--force-sdl")) ForceSDL = true;
			if(args.Contains("--force-wayland")) ForceWayland = true;
		}
	}
}