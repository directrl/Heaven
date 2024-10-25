using Coelum.Graphics.Phoenix;
using Silk.NET.Input;

namespace Coelum.Input {
	
	public static class WindowExtensions {

		public static IReadOnlyList<IKeyboard>? GetKeyboards(this SilkWindow window)
			=> window.Input?.Keyboards ?? null;

		public static IReadOnlyList<IMouse>? GetMice(this SilkWindow window)
			=> window.Input?.Mice ?? null;
		
		public static IReadOnlyList<IGamepad>? GetGamepads(this SilkWindow window)
			=> window.Input?.Gamepads ?? null;
		
		public static IReadOnlyList<IJoystick>? GetJoysticks(this SilkWindow window)
			=> window.Input?.Joysticks ?? null;
	}
}