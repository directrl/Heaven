using System.Reflection;
using Coelum.Core;

namespace Coelum.Phoenix.Editor {
	
	public class EditorApplication : Heaven {

		public static SilkWindow MainWindow { get; private set; }
		public static PhoenixScene MainScene { get; private set; }
		
		public static Assembly TargetAssembly { get; private set; }

		public EditorApplication(Assembly assembly) : base("editor-playground") {
			TargetAssembly = assembly;
		}
		
		public override void Setup(string[] args) {
			MainWindow = SilkWindow.Create(debug: true);

			MainScene = new EditorScene();
			MainWindow.Scene = MainScene;
			
			Windows.Add(MainWindow);
		}
	}
}