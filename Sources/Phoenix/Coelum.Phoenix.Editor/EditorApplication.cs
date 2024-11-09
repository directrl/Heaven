using System.Reflection;
using Coelum.Core;
using Coelum.Debug;
using Coelum.Phoenix.Editor.UI;
using Silk.NET.GLFW;
using Silk.NET.SDL;

namespace Coelum.Phoenix.Editor {
	
	public static class EditorApplication {

	#region UI
		private static List<SilkWindow> _windows = new();
		
		internal static MainUI MainUI { get; private set; }
		internal static PrefabUI PrefabUI { get; private set; }
	#endregion
		
		public static PhoenixScene TargetScene { get; private set; }
		public static Assembly TargetAssembly { get; private set; }
		
		public static void Start(PhoenixScene targetScene, Assembly targetAssembly) {
			TargetScene = targetScene;
			TargetAssembly = targetAssembly;

			Heaven.Windows.RemoveAll(window => _windows.Contains(window));
			_windows.Clear();

			MainUI = new();
			PrefabUI = new();

			//CreateWindow(MainUI);
			CreateWindow(PrefabUI);
		}

		private static SilkWindow CreateWindow(PhoenixScene scene) {
			var window = SilkWindow.Create(debug: Debugging.Enabled);
			window.Scene = scene;
			
			_windows.Add(window);
			Heaven.Windows.Add(window);
			return window;
		}
	}
}