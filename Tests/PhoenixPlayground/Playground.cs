using Coelum.Core;
using Coelum.Core.Logging;
using Coelum.Debug;
using Coelum.Phoenix;
using PhoenixPlayground.Scenes;

namespace PhoenixPlayground {
	
	public class Playground : Heaven {

		public Playground() : base("phoenix-playground") {
			Debugging.IgnoreMissingShaderUniforms = true;
		}

		public override void Setup(string[] args) {
			var window = SilkWindow.Create(debug: true);
			var scene = new FramebufferTest();
			window.Scene = scene;
			
			Windows.Add(window);

			// if(args.Contains("multi-window-test")) {
			// 	var scene1 = new InstancingTest();
			// 	var window1 = SilkWindow.Create(debug: true);
			// 	window1.Scene = scene1;
			//
			// 	Windows.Add(window1);
			// 	
			// 	var scene2 = new NodeGraphTest();
			// 	var window2 = SilkWindow.Create(debug: true);
			// 	window2.Scene = scene2;
			//
			// 	Windows.Add(window2);
			// }
		}
	}
}