using System.Reflection;
using Coelum.Core;
using Coelum.Debug;
using Coelum.Phoenix.Editor.UI;
using Coelum.Resources;
using Silk.NET.GLFW;
using Silk.NET.SDL;

namespace Coelum.Phoenix.Editor {
	
	public class EditorApplication : Heaven {
		
		public static Assembly TargetAssembly { get; private set; }

		private static PhoenixScene _targetScene;
		public static PhoenixScene TargetScene {
			get => _targetScene;
			set {
				_targetScene = value;
				MainWindow.Scene = value;
			}
		}
		
		public static SilkWindow MainWindow { get; private set; }
		public static EditorScene MainScene { get; private set; }
		
		public static EditorKeyBindings KeyBindings { get; internal set; }

		public static ResourceManager EditorResources { get; private set; }
		
		public EditorApplication(Assembly assembly, PhoenixScene scene) : base(CurrentApplication.Id) {
			TargetAssembly = assembly;
			_targetScene = scene;

			EditorResources = new(GetType().Assembly.GetName().Name + ".Resources", GetType().Assembly);
			AppResources = new(assembly.GetName().Name + ".Resources", assembly);
		}

		public override void Setup(string[] args) {
			MainWindow = SilkWindow.Create(debug: Debugging.Enabled);
			MainWindow.Scene = MainScene = new();
			
			Windows.Add(MainWindow);
		}
	}
}