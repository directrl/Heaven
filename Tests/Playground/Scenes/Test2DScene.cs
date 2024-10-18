using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics;
using Coeli.Graphics.Camera;
using Coeli.Graphics.Object;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Scene;
using Coeli.Graphics.Texture;
using Coeli.Input;
using Coeli.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class Test2DScene : Scene2D {

		private ImGuiOverlay _overlay;
		
		private Mesh? _mesh;
		private Object2D? _object;
		private Object2D? _object2;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public Test2DScene() : base("test") {
			_keyBindings = new(Id);
			//_freeCamera = new(_keyBindings);
			
			ShaderOverlays.AddRange(Texture2D.OVERLAYS);
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			Camera = new(window);

			if(_mesh == null) {
				_mesh = new(PrimitiveType.Triangles,
					new float[] {
						50f, 50f, 0.0f,
						50f, -50f, 0.0f,
						-50f, -50f, 0.0f,
						-50f, 50f, 0.0f
					},
					new uint[] {
						1, 2, 3,
						1, 2, 3
					},
					null, null);
			}

			if(_object == null && _mesh != null) {
				_object = new Object2D {
					Meshes = new[] { _mesh },
					Position = new(-50f, -50f),
					Material = new Material {
						Color = new(1.0f, 0.0f, 0.0f, 1.0f)
					}
				};

				_object2 = new Object2D {
					Meshes = new[] { _mesh },
					Position = new(50f, 50f),
					Material = new Material {
						Color = new(0.0f, 0.0f, 1.0f, 1.0f)
					}
				};
			}
			
			_overlay = new(this);
			_overlay.Render += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Object position: {_object?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Object 2 position: {_object2?.Position.ToString() ?? "Unknown"}");

					float updMs = window.UpdateDelta * 1000;
					float fxUpdMs = window.FixedUpdateDelta * 1000;
					float rndMs = window.RenderDelta * 1000;
					
					ImGui.Text($"Update delta: {updMs:F2}ms ({(1000 / updMs):F2} FPS)");
					ImGui.Text($"FixedUpdate delta: {fxUpdMs:F2}ms ({(1000 / fxUpdMs):F2} FPS)");
					ImGui.Text($"Render delta: {rndMs:F2}ms ({(1000 / rndMs):F2})");
					ImGui.End();
				}
			};

			window.Input.Keyboards[0].KeyUp += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Release, k);
			};
			
			window.Input.Keyboards[0].KeyDown += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Press, k);

				float a = 10f;
				
				if(k == Key.W && _object != null) _object.Position.Y -= a;
				if(k == Key.S && _object != null) _object.Position.Y += a;
				if(k == Key.A && _object != null) _object.Position.X -= a;
				if(k == Key.D && _object != null) _object.Position.X += a;
			};

			/*window.Input.Mice[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};*/
		}

		public override void OnUnload() {
			base.OnUnload();
			
			_keyBindings.Dispose();
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.Input.Mice[0];
			//_freeCamera.Update(Camera, ref mouse, delta);
			
			_keyBindings.Update(Window.Input.Keyboards[0]);
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			
			//MainShader.SetUniform("color", new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
			_object?.Load(PrimaryShader);
			_object?.Render();
			
			//MainShader.SetUniform("color", new Vector4(0.0f, 0.0f, 1.0f, 1.0f));
			_object2?.Load(PrimaryShader);
			_object2?.Render();
		}
	}
}