using Coeli.Core;
using Coeli.Core.Logging;
using Coeli.Debug;
using Coeli.Graphics;
using Playground.Scenes;

namespace Playground {
	
	public class Playground : Heaven {

		public Playground() : base("playground") {
			Debugging.IgnoreMissingShaderUniforms = true;
		}

		public override void Setup() {
			var scene = new Test3DScene();
			var window = Window.Create(debug: true);
			window.Scene = scene;
			
			Windows.Add(window);
		}
	}
}