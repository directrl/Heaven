using System.Diagnostics;
using Coelum.Common.Graphics;

namespace Coelum.Raven {
	
	public class ConsoleWindow : WindowBase {

		private static bool _windowExists = false;

		public Stopwatch DeltaTimer { get; } = new();

		public float FramesPerSecond { get; set; } = 60;
		
		protected ConsoleWindow() { }
		
		public override bool Update() {
			if(!DeltaTimer.IsRunning) DeltaTimer.Start();
			
			float delta = (float) DeltaTimer.Elapsed.TotalSeconds;
			float targetDelta = 1000 / FramesPerSecond / 1000;
			
			if(delta >= targetDelta) {
				Scene?.OnUpdate(delta);
				Scene?.OnRender(delta);
				DeltaTimer.Restart();
			} else {
				Thread.Sleep((int) (targetDelta - delta) * 1000 + 1);
			}
			
			return true;
		}
		
		public override void Show() {
			DeltaTimer.Start();
			Console.TreatControlCAsInput = true;
		}
		
		public override void Hide() {
			DoUpdates = false;
		}

		public static ConsoleWindow Create() {
			if(_windowExists) {
				throw new InvalidOperationException(
					"There can only be one ConsoleWindow instance globally");
			}

			_windowExists = true;
			return new();
		}
	}
}