using Coelum.Core;

namespace Coelum.Phoenix.Editor {
	
	public class EditorApplication : Heaven {

		public static SilkWindow MainWindow { get; private set; }
		public static PhoenixScene MainScene { get; private set; }
		
		public EditorApplication() : base("editor-playground") { }
		
		public override void Setup(string[] args) {
			MainWindow = SilkWindow.Create(debug: true);

			MainScene = new EditorScene();
			MainWindow.Scene = MainScene;
			
			Windows.Add(MainWindow);
		}
	}
}