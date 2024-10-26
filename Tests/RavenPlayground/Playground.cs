using Coelum.Core;
using Coelum.Raven.Window;
using RavenPlayground.Scenes;

namespace RavenPlayground {
	
	public class Playground : Heaven {
		
		public Playground() : base("playground") { }

		public override void Setup(string[] args) {
			var window = ConsoleWindow.Create();
			//var terminal = new AnsiTerminal();
			var scene = new TestBasicScene();

			window.Scene = scene;
			Windows.Add(window);
		}
	}
}