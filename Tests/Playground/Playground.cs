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

		public override void Setup() {
			var scene = new NodeGraphTest();
			var window = Window.Create(debug: true);
			window.Scene = scene;
			
			Windows.Add(window);
		}
	}
}