using Coeli.Graphics;
using Coeli.Graphics.Scene;

namespace Coeli.Input {
	
	public static class SceneExtensions {
		
		public static void SetupKeyBindings(this SceneBase scene, KeyBindings keyBindings) {
			if(scene.Window == null) {
				scene.Load += window => DoSetupKeyBindings(window, keyBindings);
			} else {
				DoSetupKeyBindings(scene.Window, keyBindings);
			}

			scene.Unload += keyBindings.Dispose;
		}

		public static void UpdateKeyBindings(this SceneBase scene, KeyBindings keyBindings) {
			if(scene.Window?.Input == null) return;
			
			foreach(var keyboard in scene.Window.Input.Keyboards) {
				keyBindings.Update(keyboard);
			}
		}

		private static void DoSetupKeyBindings(Window window, KeyBindings keyBindings) {
			if(window.Input == null) throw new InvalidOperationException("Window input not initialized");
			
			foreach(var keyboard in window.Input.Keyboards) {
				keyboard.KeyUp += (_, key, _) => keyBindings.Input(KeyAction.Release, key);
				keyboard.KeyDown += (_, key, _) => keyBindings.Input(KeyAction.Press, key);
			}
		}
	}
}