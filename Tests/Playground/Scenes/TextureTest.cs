using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics;
using Coeli.Graphics.Camera;
using Coeli.Graphics.Object;
using Coeli.Graphics.OpenGL;
using Coeli.Graphics.Scene;
using Coeli.Graphics.Texture;
using Coeli.Input;
using Coeli.Resources;
using Coeli.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class TextureTest : Scene3D {

		private ImGuiOverlay _overlay;
		
		private Mesh? _mesh;
		private Object3D? _object;
		private InstancedObject<Object3D>? _instObject;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public TextureTest() : base("texture-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
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
						-0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f,  0.5f, -0.5f,
						0.5f,  0.5f, -0.5f,
						-0.5f,  0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,

						-0.5f, -0.5f,  0.5f,
						0.5f, -0.5f,  0.5f,
						0.5f,  0.5f,  0.5f,
						0.5f,  0.5f,  0.5f,
						-0.5f,  0.5f,  0.5f,
						-0.5f, -0.5f,  0.5f,

						-0.5f,  0.5f,  0.5f,
						-0.5f,  0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f,  0.5f,
						-0.5f,  0.5f,  0.5f,

						0.5f,  0.5f,  0.5f,
						0.5f,  0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f,  0.5f,
						0.5f,  0.5f,  0.5f,

						-0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f,  0.5f,
						0.5f, -0.5f,  0.5f,
						-0.5f, -0.5f,  0.5f,
						-0.5f, -0.5f, -0.5f,

						-0.5f,  0.5f, -0.5f,
						0.5f,  0.5f, -0.5f,
						0.5f,  0.5f,  0.5f,
						0.5f,  0.5f,  0.5f,
						-0.5f,  0.5f,  0.5f,
						-0.5f,  0.5f, -0.5f,
					},
					new uint[] {
						0, 1, 2, 3, 4, 5,
						6, 7, 8, 9, 10, 11,
						12, 13, 14, 15, 16, 17,
						18, 19, 20, 21, 22, 23,
						24, 25, 26, 27, 28, 29,
						30, 31, 32, 33, 34, 35
					},
					new float[] {
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
				
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
				
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
				
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
				
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
				
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
					}, null);
			}

			if(_instObject == null && _mesh != null) {
				_object = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 0, 0),
					Material = new() {
						Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "one"]),
						Color = new(1, 1, 1, 1)
					}
				};
				
				/*int wall = 32;

				_instObject = new((int) Math.Pow(wall, 3)) {
					Meshes = new Mesh[] { _mesh }
				};

				var tex1 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "one"]);
				var tex2 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "two"]);
				var tex3 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "three"]);
				var tex4 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "four"]);
				
				for(int y = 0; y < (wall * 2); y += 2)
				for(int x = 0; x < (wall * 2); x += 2)
				for(int z = 0; z < (wall * 2); z += 2) {
					var o = new Object3D {
						Meshes = new[] { _mesh },
						Position = new(x, y, z)
					};

					int i = x + y + z;

					if(i % 4 == 0) {
						o.Material = new Material {
							Color = new(1, 1, 1, 1),
							Texture = tex1
						};
					} else if(i % 4 == 1) {
						o.Material = new Material {
							Color = new(1, 1, 1, 1),
							Texture = tex2
						};
					} else if(i % 4 == 2) {
						o.Material = new Material {
							Color = new(1, 1, 1, 1),
							Texture = tex3
						};
					} else if(i % 4 == 3) {
						o.Material = new Material {
							Color = new(1, 1, 1, 1),
							Texture = tex4
						};
					}
				}*/
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

			window.GetMice()[0].MouseMove += (mouse, pos) => {
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
			
			this.UpdateKeyBindings(_keyBindings);
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			gl.Disable(EnableCap.CullFace);
			
			_object?.Render(MainShader);
		}
	}
}