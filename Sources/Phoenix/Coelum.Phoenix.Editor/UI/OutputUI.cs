using System.Numerics;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;

namespace Coelum.Phoenix.Editor.UI {
	
	public class OutputUI : ImGuiUI {

		private readonly OutputScene _output;
		private Vector2 _lastSize;
		private bool _noMove;

		private ImGuizmoOperation _gizmoOperation = ImGuizmoOperation.Translate;
		public ImGuizmoOperation GizmoOperation {
			get {
				if(!_output._editor) throw new NotSupportedException();
				return _gizmoOperation;
			}
			set {
				if(!_output._editor) throw new NotSupportedException();
				_gizmoOperation = value;
			}
		}

		public OutputUI(PhoenixScene scene, OutputScene output) : base(scene) {
			_output = output;
		}

		public unsafe override void Render(float delta) {
			if(_output._editor) {
				
			}
			
			ImGui.Begin(_output.Id, _noMove ? ImGuiWindowFlags.NoMove : ImGuiWindowFlags.None);
			{
			#region Framebuffer render
				var size = ImGui.GetContentRegionAvail();

				if(size != _lastSize && size is { X: > 0, Y: > 0 }) {
					_output.OutputFramebuffer.Size = new((int) size.X, (int) size.Y);
					_lastSize = size;
				}
				
				ImGui.Image(
					new ImTextureID(_output.OutputFramebuffer.Texture.Id),
					size,
					new Vector2(0, 1), new Vector2(1, 0)
				);
			#endregion

			#region Gizmos
				if(_output._editor) {
					var sn = EditorApplication.MainScene.NodeUI.SelectedNode;
					var fc = EditorApplication.MainScene.EditorView.FreeCamera;
				
					if(sn != null && fc != null
					   && sn.TryGetComponent<Transform, Transform3D>(out var t3d)) {
					#region ImGuizmo setup
						ImGuizmo.SetImGuiContext(Controller.Context);
						ImGuizmo.BeginFrame();
						ImGuizmo.SetOrthographic(false);
						ImGuizmo.SetDrawlist(ImGui.GetWindowDrawList());
						ImGuizmo.Enable(true);
						
						var wPos = ImGui.GetWindowPos();
						var wSize = ImGui.GetWindowSize();
						
						ImGuizmo.SetRect(wPos.X, wPos.Y, wSize.X, wSize.Y);
					#endregion
						
						var cameraView = fc.Camera.ViewMatrix;
						var cameraProjection = fc.Camera.ProjectionMatrix;
						var nodeGlobalMatrix = t3d.GlobalMatrix;
						
						ImGuizmo.Manipulate(&cameraView.M11, &cameraProjection.M11,
						                    GizmoOperation, ImGuizmoMode.Local,
						                    &nodeGlobalMatrix.M11);
						
						// TODO
						// ImGuizmo.ViewManipulate(&vm.M11,
						//                         100,
						//                         wPos + new Vector2(20, 20),
						//                         new(100, 100),
						//                         0);

						_noMove = ImGuizmo.IsOver();

						if(ImGuizmo.IsUsing()) {
							var newNodeMatrix = new Matrix4x4();

							if(sn.Parent != null &&
							   sn.Parent.TryGetComponent<Transform, Transform3D>(out var pt)) {
								Matrix4x4.Invert(pt.GlobalMatrix, out var parentGlobalMatrix);
								newNodeMatrix = parentGlobalMatrix * nodeGlobalMatrix;
							} else {
								newNodeMatrix = nodeGlobalMatrix;
							}
							
							var translationMatrix = new Matrix4x4();
							var rotationMatrix = new Matrix4x4();
							var scaleMatrix = new Matrix4x4();
							
							ImGuizmo.DecomposeMatrixToComponents(
								ref newNodeMatrix,
								ref translationMatrix,
								ref rotationMatrix,
								ref scaleMatrix
							);
							
							var translation = new Vector3(translationMatrix.M11, translationMatrix.M12, translationMatrix.M13);
							var rotation = new Vector3(rotationMatrix.M11, rotationMatrix.M12, rotationMatrix.M13);
							var scale = new Vector3(scaleMatrix.M11, scaleMatrix.M12, scaleMatrix.M13);
							
							Matrix4x4.Decompose(newNodeMatrix, out var s, out var r, out var t);

							switch(GizmoOperation) {
								case ImGuizmoOperation.Translate:
									t3d.Position = t;
									break;
								case ImGuizmoOperation.Rotate:
									t3d.Rotation = r.ToPitchYawRoll(); // TODO switch to quaternions
									break;
								case ImGuizmoOperation.Scale:
									t3d.Scale = s;
									break;
							}
						}
					}
				}
			#endregion
			}
			ImGui.End();
		}
	}
}