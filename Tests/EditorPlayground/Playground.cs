using Coelum.Core;
using Coelum.Debug;
using Coelum.Phoenix;
using Coelum.Phoenix.Editor;
using EditorPlayground.Scenes;

namespace EditorPlayground {
	
	public class Playground : Heaven {
		
		public Playground() : base("editor-playground") { }

		public override void Setup(string[] args) {
			var window = SilkWindow.Create(debug: Debugging.Enabled);
			window.Scene = new EmptyScene();
			
			Windows.Add(window);
			
			EditorApplication.Start((PhoenixScene) window.Scene, GetType().Assembly);
		}
	}
}