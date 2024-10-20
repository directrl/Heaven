using Coelum.Core;
using Coelum.Core.Logging;
using Coelum.Debug;
using Coelum.Graphics;
using Playground.Scenes;

namespace Playground {
	
	public class Playground : Heaven {

		public Playground() : base("playground") {
			Debugging.IgnoreMissingShaderUniforms = true;
		}

		public override void Setup(string[] args) {
			var scene = new TextureTest();
			var window = Window.Create(debug: true);
			window.Scene = scene;
			
			Windows.Add(window);

			if(args.Contains("multi-window-test")) {
				var scene1 = new InstancingTest();
				var window1 = Window.Create(debug: true);
				window1.Scene = scene1;
			
				Windows.Add(window1);
			}
		}
	}
}