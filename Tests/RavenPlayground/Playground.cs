using Coelum.Core;
using Coelum.Raven;
using Coelum.Raven.Terminal;
using RavenPlayground.Scenes;

namespace RavenPlayground {
	
	public class Playground : Heaven {
		
		public Playground() : base("playground") { }

		public override void Setup(string[] args) {
			var window = ConsoleWindow.Create();
			var terminal = new AnsiTerminal();
			var scene = new TestBasicScene(terminal);

			window.Scene = scene;
			Windows.Add(window);
		}
	}
}