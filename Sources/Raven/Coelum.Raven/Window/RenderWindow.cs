using System.Diagnostics;
using Coelum.Common.Graphics;
using Coelum.Raven.Display;
using Coelum.Raven.Scene;
using Serilog;

namespace Coelum.Raven.Window {
	
	public class RenderWindow : WindowBase {
		
		public Stopwatch DeltaTimer { get; } = new();
		public float Delta { get; private set; }
		public float TargetDelta => 1000 / FramesPerSecond;
		
		public float FramesPerSecond { get; set; } = 60;
		
		public RenderContext Context { get; }

		public RenderWindow(RenderContext ctx) {
			Context = ctx;
		}
		
		public override bool Update() {
			Delta = (float) DeltaTimer.Elapsed.TotalMilliseconds;
			DeltaTimer.Restart();
			
			try {
				Scene?.OnUpdate(Delta);

				var cc = RenderContext.DEFAULT_CELL;
				if(Scene is not null && Scene is RavenSceneBase ravenScene) {
					cc = ravenScene.ClearCell;
				}
				
				Context.Clear(ref Context.BackBuffer, cc);
				Scene?.OnRender(Delta);
				Context.Render();
			} catch(Exception e) {
				try {
					Scene = new ExceptionScene(e);

					Scene?.OnUpdate(Delta);
					Context.Clear(ref Context.BackBuffer);
					Scene?.OnRender(Delta);
					Context.Render();
				} catch(Exception e1) {
					while(true) {
						Log.Fatal("DOUBLE FAULT");
						Log.Fatal($"e0: {e.Source} {e.Message}\n{e.StackTrace}");
						Log.Fatal($"e1: {e1.Source} {e1.Message}\n{e1.StackTrace}");
						
						Console.ReadKey();
						Thread.Sleep(1000);
					}
				}
			}
			
			double elapsed = DeltaTimer.Elapsed.TotalMilliseconds;
			double sleepTime = TargetDelta - elapsed;

			if(sleepTime > 0) {
				Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime));
			}
			
			return true;
		}
		
		public override void Show() {
			DeltaTimer.Start();
		}
		
		public override void Hide() {
			DoUpdates = false;
		}
	}
}