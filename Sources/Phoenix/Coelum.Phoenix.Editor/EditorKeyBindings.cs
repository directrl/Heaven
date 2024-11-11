using Coelum.Common.Input;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;

namespace Coelum.Phoenix.Editor {
	
	public class EditorKeyBindings {
		
		public KeyBinding FreeCameraEngage { get; }
		
		public KeyBinding OperationTranslate { get; }
		public KeyBinding OperationRotate { get; }
		public KeyBinding OperationScale { get; }

		public EditorKeyBindings(KeyBindings keyBindings) {
			FreeCameraEngage = keyBindings.Register(new("fc_engage", Key.ControlLeft, Key.E));
			
			OperationTranslate = keyBindings.Register(new("g_op_translate", Key.ControlLeft, Key.G));
			OperationRotate = keyBindings.Register(new("g_op_rotate", Key.ControlLeft, Key.R));
			OperationScale = keyBindings.Register(new("g_op_scale", Key.ControlLeft, Key.V));
		}

		public void Update(float delta) {
			if(OperationTranslate.Pressed) {
				EditorApplication.MainScene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Translate;
			} else if(OperationRotate.Pressed) {
				EditorApplication.MainScene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Rotate;
			} else if(OperationScale.Pressed) {
				EditorApplication.MainScene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Scale;
			}
		}
	}
}