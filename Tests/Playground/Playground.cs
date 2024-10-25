using Coelum.Core;
using Coelum.Core.Logging;
using Coelum.Debug;
using Coelum.Graphics.Phoenix;
using Playground.Scenes;

namespace Playground {
	
	public class Playground : Heaven {

		public Playground() : base("playground") {
			Debugging.IgnoreMissingShaderUniforms = true;
		}

		public override void Setup(string[] args) {
			var scene = new ModelTest();
			var window = SilkWindow.Create(debug: true);
			window.Scene = scene;
			
			Windows.Add(window);

			if(args.Contains("multi-window-test")) {
				var scene1 = new InstancingTest();
				var window1 = SilkWindow.Create(debug: true);
				window1.Scene = scene1;
			
				Windows.Add(window1);
				
				var scene2 = new NodeGraphTest();
				var window2 = SilkWindow.Create(debug: true);
				window2.Scene = scene2;
			
				Windows.Add(window2);
			}
		}
	}
}