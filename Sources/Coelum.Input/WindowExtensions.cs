using Coelum.Graphics;
using Silk.NET.Input;

namespace Coelum.Input {
	
	public static class WindowExtensions {

		public static IReadOnlyList<IKeyboard>? GetKeyboards(this Window window)
			=> window.Input?.Keyboards ?? null;

		public static IReadOnlyList<IMouse>? GetMice(this Window window)
			=> window.Input?.Mice ?? null;
		
		public static IReadOnlyList<IGamepad>? GetGamepads(this Window window)
			=> window.Input?.Gamepads ?? null;
		
		public static IReadOnlyList<IJoystick>? GetJoysticks(this Window window)
			=> window.Input?.Joysticks ?? null;
	}
}