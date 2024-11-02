using Coelum.Common.Input;
using Coelum.Phoenix;

namespace Coelum.Phoenix.Input {
	
	public static class SceneExtensions {
		
		public static void SetupKeyBindings(this PhoenixScene scene, KeyBindings keyBindings) {
			if(scene.Window == null) {
				scene.Load += window => DoSetupKeyBindings((SilkWindow) window, keyBindings);
			} else {
				DoSetupKeyBindings(scene.Window, keyBindings);
			}

			scene.Unload += keyBindings.Dispose;
		}

		public static void UpdateKeyBindings(this PhoenixScene scene, KeyBindings keyBindings) {
			if(scene.Window?.Input == null) return;
			
			foreach(var keyboard in scene.Window.Input.Keyboards) {
				keyBindings.Update(new SilkKeyboard(keyboard));
			}
		}

		private static void DoSetupKeyBindings(SilkWindow window, KeyBindings keyBindings) {
			if(window.Input == null) throw new InvalidOperationException("Window input not initialized");
			
			foreach(var keyboard in window.Input.Keyboards) {
				keyboard.KeyUp += (_, key, _) => keyBindings.Input(KeyAction.Release, (int) key);
				keyboard.KeyDown += (_, key, _) => keyBindings.Input(KeyAction.Press, (int) key);
			}
		}
	}
}