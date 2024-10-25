using Coelum.Input.Common;
using Coelum.Phoenix;
using Coelum.Phoenix.Scene;

namespace Coelum.Phoenix.Input {
	
	public static class SceneExtensions {
		
		public static void SetupKeyBindings(this PhoenixSceneBase scene, KeyBindings keyBindings) {
			if(scene.Window == null) {
				scene.Load += window => DoSetupKeyBindings((SilkWindow) window, keyBindings);
			} else {
				DoSetupKeyBindings(scene.Window, keyBindings);
			}

			scene.Unload += keyBindings.Dispose;
		}

		public static void UpdateKeyBindings(this PhoenixSceneBase scene, KeyBindings keyBindings) {
			if(scene.Window?.Input == null) return;
			
			foreach(var keyboard in scene.Window.Input.Keyboards) {
				keyBindings.Update(keyboard);
			}
		}

		private static void DoSetupKeyBindings(SilkWindow window, KeyBindings keyBindings) {
			if(window.Input == null) throw new InvalidOperationException("Window input not initialized");
			
			foreach(var keyboard in window.Input.Keyboards) {
				keyboard.KeyUp += (_, key, _) => keyBindings.Input(KeyAction.Release, key);
				keyboard.KeyDown += (_, key, _) => keyBindings.Input(KeyAction.Press, key);
			}
		}
	}
}