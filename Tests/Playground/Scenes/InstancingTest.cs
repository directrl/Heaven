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
	
	public class InstancingTest : Scene3D {

		private DebugUI _overlay;
		
		private Mesh? _mesh;
		private InstancedObject<Object3D>? _instObject;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public InstancingTest() : base("instancing") {
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

			if(_instObject == null && _mesh != null) {
				List<Object3D> objects = new();
				List<Matrix4x4> models = new();

				int wall = 128;
				
				for(int y = 0; y < (wall * 2); y += 2)
				for(int x = 0; x < (wall * 2); x += 2)
				for(int z = 0; z < (wall * 2); z += 2) {
					var o = new Object3D {
						Position = new(x, y, z)
					};
					
					objects.Add(o);
					models.Add(o.ModelMatrix);
				}
				
				Playground.AppLogger.Information($"Created objects: {objects.Count}");

				_instObject = new(objects.ToArray(), models.ToArray()) {
					Meshes = new[] { _mesh }
				};
				
				Playground.AppLogger.Information("Building instances");
				_instObject.Build();
				Playground.AppLogger.Information($"OK: {_instObject.ObjectCount}");
			}
			
			_overlay = new(this);
			_overlay.AdditionalInfo += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Camera position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");
					ImGui.Text($"Object count: {_instObject?.ObjectCount.ToString() ?? "Unknown"}");
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
			
			MainShader.SetUniform("color", new Vector4(1.0f, 1.0f, 1.0f, 1.0f));
			_instObject?.Render();
		}
	}
}