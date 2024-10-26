using System.Diagnostics;
using Coelum.Common.Graphics;
using Coelum.Raven.Display;

namespace Coelum.Raven.Window {
	
	public class ConsoleWindow : RenderWindow {

		private static bool _windowExists = false;

		protected ConsoleWindow()
			: base(new(new AnsiDisplay(Console.BufferWidth, Console.BufferHeight, Console.Out))) {

			_windowExists = true;
			Console.TreatControlCAsInput = true;
		}

		public static ConsoleWindow Create() {
			if(_windowExists) {
				throw new InvalidOperationException(
					"There can only be one ConsoleWindow instance globally");
			}

			return new();
		}
	}
}