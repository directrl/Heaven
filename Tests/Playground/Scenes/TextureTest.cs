using System.Numerics;
using Coelum.Debug;
using Coelum.Resources;
using Coelum.Graphics;
using Coelum.Graphics.Camera;
using Coelum.Graphics.Node;
using Coelum.Graphics.Scene;
using Coelum.Graphics.Texture;
using Coelum.Input;
using Coelum.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class TextureTest : Scene3D {
		
		private static readonly Random RANDOM = new();

		private ImGuiOverlay _overlay;
		
		private Mesh? _mesh;
		
		/*private Material? _mat1;
		private Material? _mat2;
		private Material? _mat3;
		private Material? _mat4;
		
		private Object3D? _object1;
		private Object3D? _object2;
		private Object3D? _object3;
		private Object3D? _object4;*/

		private Node3D? _object;
		private List<Node3D>? _objects;
		private InstancedNode<Node3D>? _instObject;
		private TextureArray? _texArray;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		public TextureTest() : base("texture-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Texture2D.OVERLAYS);
			ShaderOverlays.AddRange(TextureArray.OVERLAYS);
			ShaderOverlays.AddRange(InstancedNode<Node3D>.OVERLAYS);
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

			if(_texArray == null) {
				var textures = new Resource[] {
					Playground.AppResources[Resource.Type.TEXTURE, "one"],
					Playground.AppResources[Resource.Type.TEXTURE, "two"],
					Playground.AppResources[Resource.Type.TEXTURE, "three"],
					Playground.AppResources[Resource.Type.TEXTURE, "four"]
				};
				
				_texArray = TextureArray.Create(null, textures);
			}

			if(_instObject == null && _mesh != null) {
				_objects = new();
				
				/*_mat1 = new() {
					//Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "one"]),
					TextureLayer = 0,
					Color = new(1, 1, 1, 1)
				};
				_mat2 = new() {
					//Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "two"]),
					TextureLayer = 1,
					Color = new(1, 1, 1, 1)
				};
				_mat3 = new() {
					//Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "three"]),
					TextureLayer = 2,
					Color = new(1, 1, 1, 1)
				};
				_mat4 = new() {
					//Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "four"]),
					TextureLayer = 3,
					Color = new(1, 1, 1, 1)
				};*/
				
				/*_object1 = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 0, 0),
					Material = _mat1.Value
				};
				_object2 = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 2, 0),
					Material = _mat2.Value
				};
				_object3 = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 4, 0),
					Material = _mat3.Value
				};
				_object4 = new Object3D {
					Meshes = new[] { _mesh },
					Position = new(0, 6, 0),
					Material = _mat4.Value
				};*/

				_object = new Node3D() {
					Position = new(0, 20, 0),
					Model = new() {
						Meshes = new[] { _mesh },
						Material = new() {
							Color = new(1, 1, 1, 1),
							Texture = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "two"])
						}
					}
				};
				
				int wall = 64;

				_instObject = new(new(_mesh), (int) Math.Pow(wall, 3));

				// var tex1 = Texture2D.Load();
				// var tex2 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "two"]);
				// var tex3 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "three"]);
				// var tex4 = Texture2D.Load(Playground.AppResources[Resource.Type.TEXTURE, "four"]);
				
				for(int y = 0; y < (wall * 2); y += 2)
				for(int x = 0; x < (wall * 2); x += 2)
				for(int z = 0; z < (wall * 2); z += 2) {
					int random = RANDOM.Next(0, 4);
					
					var o = new Node3D() {
						Position = new(x, y, z),
						Model = new() {
							Meshes = new[] { _mesh },
							Material = new() {
								Color = new(1, 1, 1, 1),
								TextureLayer = random
							}
						}
					};
					
					_instObject.Add(o);
					_objects.Add(o);
				}
				
				_instObject.Build();
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
			
			_texArray?.Bind();
			
			// foreach(var o in _objects) {
			// 	o.Render(PrimaryShader);
			// }
			
			_instObject?.Render(PrimaryShader);
			
			//_object?.Render(PrimaryShader);
		}
	}
}