using System.Numerics;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Nodes;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;
using Silk.NET.Input;
using Silk.NET.Maths;

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
		
		private ImGuizmoMode _gizmoMode = ImGuizmoMode.World;
		public ImGuizmoMode GizmoMode {
			get {
				if(!_output._editor) throw new NotSupportedException();
				return _gizmoMode;
			}
			set {
				if(!_output._editor) throw new NotSupportedException();
				_gizmoMode = value;
			}
		}

		private SingleHitHandler _rayCast = new();
		public SingleHitHandler RayCast {
			get {
				if(!_output._editor) throw new NotSupportedException();
				return _rayCast;
			}
			set {
				if(!_output._editor) throw new NotSupportedException();
				_rayCast = value;
			}
		}

		public OutputUI(PhoenixScene scene, OutputScene output) : base(scene) {
			_output = output;

			/*if(_output._editor) {
				scene.Window!.GetMice()[0].Click += (mouse, button, pos) => {
					if(button != MouseButton.Left) return;
				
					if(RayCast.Result.Hit) {
						EditorApplication.MainScene.NodeUI.SelectedNode = RayCast.Result.PhysicsNode;
					}
				};
			}*/
		}

		public unsafe override void Render(float delta) {
			ImGui.Begin(_output.Id, _noMove ? ImGuiWindowFlags.NoMove : ImGuiWindowFlags.None);
			{
			#region Framebuffer render
				var size = ImGui.GetContentRegionAvail();

				if(size != _lastSize && size is { X: > 0, Y: > 0 }) {
					_output.OutputFramebuffer.Size = new((int) size.X, (int) size.Y);
					_lastSize = size;
				}
				
				ImGui.Image(
					new ImTextureID(_output.OutputFramebuffer.Texture.Handle),
					size,
					new Vector2(0, 1), new Vector2(1, 0)
				);
			#endregion

			#region Gizmos
				if(_output._editor) {
					var fc = EditorApplication.MainScene.EditorView.FreeCamera;

				#region Selection raycast
					if(!fc!.Active && EditorApplication.KeyBindings.NodePicker.Down) {
						foreach(var (id, e) in SimulationManager.Simulations) {
							var simulation = e.Item1;

							var mousePos = EditorApplication.MainScene.Window!.Input.Mice[0].Position;
							mousePos.X -= ImGui.GetWindowPos().X;
							mousePos.Y -= ImGui.GetWindowPos().Y;
						
							RayCast = simulation
								.RayCast<SingleHitHandler>(fc.Camera, EditorApplication.MainScene.EditorView.OutputViewport, mousePos);

							if(RayCast.Result.Hit) break;
						}
						
						if(RayCast.Result.Hit) {
							EditorApplication.MainScene.NodeUI.SelectedNode = RayCast.Result.PhysicsNode;
						}
					} else {
						RayCast = new();
					}
				#endregion
					
					var sn = EditorApplication.MainScene.NodeUI.SelectedNode;
				
					if(sn != null && fc != null
					   && sn.TryGetComponent<Transform, Transform3D>(out var t3d)) {
					#region ImGuizmo setup
						ImGuizmo.SetImGuiContext(Controller.Context);
						ImGuizmo.BeginFrame();
						ImGuizmo.SetOrthographic(fc.Camera is OrthographicCamera);
						ImGuizmo.SetDrawlist(ImGui.GetWindowDrawList());
						ImGuizmo.Enable(true);
						
						var wPos = ImGui.GetWindowPos();
						var wSize = ImGui.GetWindowSize();
						
						ImGuizmo.SetRect(wPos.X, wPos.Y, wSize.X, wSize.Y);
					#endregion
						
						var cameraView = fc.Camera.ViewMatrix;
						var cameraProjection = fc.Camera.ProjectionMatrix;
						var nodeGlobalMatrix = t3d.GlobalMatrix;

						// undo offset to render gizmo at correct origin
						nodeGlobalMatrix = Matrix4x4.CreateTranslation(-t3d.Offset) * nodeGlobalMatrix;
						
						ImGuizmo.Manipulate(&cameraView.M11, &cameraProjection.M11,
						                    GizmoOperation, GizmoMode,
						                    &nodeGlobalMatrix.M11);
						
						// TODO
						// ImGuizmo.ViewManipulate(&cameraView.M11,
						//                         100,
						//                         wPos + new Vector2(20, 20),
						//                         new(100, 100),
						//                         0);

						_noMove = ImGuizmo.IsOver();

						if(ImGuizmo.IsUsing()) {
							bool hasPhysicsBody = false;
							if(sn.TryGetComponent<PhysicsBody>(out var physicsBody)) {
								hasPhysicsBody = true;
								physicsBody.DoUpdates = false;
							}

							var newNodeMatrix = nodeGlobalMatrix;

							if(sn.Parent != null &&
							   sn.Parent.TryGetComponent<Transform, Transform3D>(out var pt)) {
								Matrix4x4.Invert(pt.GlobalMatrix, out var parentGlobalMatrix);
								
								// for whatever reason translation requires * while everything needs +
								// weird hack, but if it works it's not stupid
								newNodeMatrix =
									GizmoOperation == ImGuizmoOperation.Translate
									? parentGlobalMatrix + nodeGlobalMatrix
									: parentGlobalMatrix * nodeGlobalMatrix;
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
							var rotation = new Vector3(rotationMatrix.M11.ToRadians(), rotationMatrix.M12.ToRadians(), rotationMatrix.M13.ToRadians());
							var scale = new Vector3(scaleMatrix.M11, scaleMatrix.M12, scaleMatrix.M13);

							switch(GizmoOperation) {
								case ImGuizmoOperation.Translate:
									t3d.Position = translation;
									break;
								case ImGuizmoOperation.Rotate:
									t3d.Rotation = rotation;
									break;
								case ImGuizmoOperation.Scale:
									t3d.Scale = scale;
									break;
							}

							if(hasPhysicsBody) {
								physicsBody.DoUpdates = true;
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