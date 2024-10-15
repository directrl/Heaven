using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics;
using Coeli.Graphics.Camera;
using Coeli.Graphics.Object;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Scene;
using Coeli.Input;
using Coeli.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class Test3DScene : Scene3D {

		private ImGuiOverlay _overlay;
		
		private Mesh? _mesh;
		private Object3D? _object;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public Test3DScene() : base("test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			if(Camera == null) {
				Camera = new PerspectiveCamera(window) {
					FOV = 60,
					Position = new(0, 0, 0)
				};
			}

			if(_mesh == null) {
				_mesh = new(PrimitiveType.Triangles,
					new float[] {
						0.5f, 0.5f, 0.0f,
						0.5f, -0.5f, 0.0f,
						-0.5f, -0.5f, 0.0f,
						-0.5f, 0.5f, 0.0f
					},
					new uint[] {
						1, 2, 3,
						1, 2, 3
					},
					null, null);
				
				_mesh = new Mesh(PrimitiveType.Triangles,
					new float[] {
						// VO
						-0.5f, 0.5f, 0.5f,
						// V1
						-0.5f, -0.5f, 0.5f,
						// V2
						0.5f, -0.5f, 0.5f,
						// V3
						0.5f, 0.5f, 0.5f,
						// V4
						-0.5f, 0.5f, -0.5f,
						// V5
						0.5f, 0.5f, -0.5f,
						// V6
						-0.5f, -0.5f, -0.5f,
						// V7
						0.5f, -0.5f, -0.5f,
					},
					new uint[] {
						// Front face
						0, 1, 3, 3, 1, 2,
						// Top Face
						4, 0, 3, 5, 4, 3,
						// Right face
						3, 2, 7, 5, 3, 7,
						// Left face
						6, 1, 0, 6, 0, 4,
						// Bottom face
						2, 1, 6, 2, 6, 7,
						// Back face
						7, 6, 4, 7, 4, 5,
					},
					null,
					null
				);
			}

			if(_object == null && _mesh != null) {
				_object = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 0, 0)
				};
			}
			
			_overlay = new(this);
			_overlay.Render += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Camera position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");
					ImGui.End();
				}
			};

			window.Input.Keyboards[0].KeyUp += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Release, k);
			};
			
			window.Input.Keyboards[0].KeyDown += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Press, k);
			};

			window.Input.Mice[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUnload() {
			base.OnUnload();
			
			_keyBindings.Dispose();
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.Input.Mice[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			_keyBindings.Update(Window.Input.Keyboards[0]);
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			
			//MainShader.SetUniform("color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
			_object?.Render(MainShader);
		}
	}
}