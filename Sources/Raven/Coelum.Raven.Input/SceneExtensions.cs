using Coelum.Common.Input;
using Coelum.Raven.Scene;
using Coelum.Raven.Window;

// TODO reading input in a non-stupid way is impossible on console applications :D
namespace Coelum.Raven.Input {
	
	public static class SceneExtensions {

		// public static void SetupKeyBindings(this RavenSceneBase scene, KeyBindings keyBindings) {
		// 	if(scene.Window == null) {
		// 		scene.Load += window => DoSetupKeyBindings((SilkWindow) window, keyBindings);
		// 	} else {
		// 		DoSetupKeyBindings(scene.Window, keyBindings);
		// 	}
		//
		// 	scene.Unload += keyBindings.Dispose;
		// }
		//
		// public static void UpdateKeyBindings(this RavenSceneBase scene, KeyBindings keyBindings) {
		// 	if(scene.Window?.Input == null) return;
		// 	
		// 	foreach(var keyboard in scene.Window.Input.Keyboards) {
		// 		keyBindings.Update(keyboard);
		// 	}
		// }
		//
		// private static void DoSetupKeyBindings(RenderWindow window, KeyBindings keyBindings) {
		// 	if(window.Input == null) throw new InvalidOperationException("Window input not initialized");
		// 	
		// 		keyboard.KeyUp += (_, key, _) => keyBindings.Input(KeyAction.Release, key);
		// 		keyboard.KeyDown += (_, key, _) => keyBindings.Input(KeyAction.Press, key);
		// }
	}
}