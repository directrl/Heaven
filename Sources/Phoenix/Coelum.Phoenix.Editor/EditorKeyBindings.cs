using Coelum.Common.Input;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;

namespace Coelum.Phoenix.Editor {
	
	public class EditorKeyBindings {
		
		public KeyBinding ClosePopup { get; }
		
		public KeyBinding FreeCameraEngage { get; }
		public KeyBinding FreeCameraDisengage { get; }
		
		public KeyBinding OperationTranslate { get; }
		public KeyBinding OperationRotate { get; }
		public KeyBinding OperationScale { get; }
		
		public KeyBinding ModeToggle { get; }
		
		public KeyBinding NodePicker { get; }
		public KeyBinding SpawnNode { get; }

		public EditorKeyBindings(KeyBindings keyBindings) {
			ClosePopup = keyBindings.Register(new("popup_close", Key.Escape));
			
			FreeCameraEngage = keyBindings.Register(new("fc_engage", Key.ControlLeft, Key.E));
			FreeCameraDisengage = keyBindings.Register(new("fc_disengage", Key.C));
			
			OperationTranslate = keyBindings.Register(new("g_op_translate", Key.ControlLeft, Key.G));
			OperationRotate = keyBindings.Register(new("g_op_rotate", Key.ControlLeft, Key.R));
			OperationScale = keyBindings.Register(new("g_op_scale", Key.ControlLeft, Key.V));

			ModeToggle = keyBindings.Register(new("g_mode", Key.ControlLeft, Key.ShiftLeft, Key.G));
			
			NodePicker = keyBindings.Register(new("node_picker", Key.X));
			SpawnNode = keyBindings.Register(new("node_spawn_new", Key.ShiftLeft, Key.A));
		}

		public void Update(float delta) {
			var scene = EditorApplication.MainScene;
			
			if(ClosePopup.Pressed) ImGui.CloseCurrentPopup();
			
			if(OperationTranslate.Pressed) {
				scene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Translate;
			} else if(OperationRotate.Pressed) {
				scene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Rotate;
			} else if(OperationScale.Pressed) {
				scene.EditorViewUI.GizmoOperation = ImGuizmoOperation.Scale;
			}

			if(ModeToggle.Pressed) {
				scene.EditorViewUI.GizmoMode =
					scene.EditorViewUI.GizmoMode == ImGuizmoMode.Local
						? ImGuizmoMode.World
						: ImGuizmoMode.Local;
			}

			if(SpawnNode.Pressed) {
				scene.NodeSpawner.Prompt(scene.NodeUI.SelectedNode);
			}
		}
	}
}