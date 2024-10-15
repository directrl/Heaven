using Coeli.Core;
using Coeli.Core.Logging;
using Coeli.Graphics;
using Playground.Scenes;

namespace Playground {
	
	public class Playground : Heaven {

		public Playground() : base("playground") { }

		public override void Setup() {
			var scene = new Test2DScene();
			var window = Window.Create(debug: true);
			window.Scene = scene;
			
			Windows.Add(window);
		}
	}
}