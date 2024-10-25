using System.Drawing;
using Coelum.Graphics;
using Coelum.Graphics.Camera;
using Coelum.Graphics.Node;
using Coelum.Graphics.Scene;
using Coelum.Graphics.Texture;
using Coelum.Input;
using Coelum.LanguageExtensions;
using Coelum.Node;
using Coelum.UI;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using IShaderRenderable = Coelum.Graphics.Node.Component.IShaderRenderable;

namespace Playground.Scenes {
	
	public class NodeGraphTest : Scene3D {
		
		private static readonly Random RANDOM = new();
		
		private ImGuiOverlay _overlay;
		
		private Mesh? _mesh;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private Camera3D _camera1;
		private Camera3D _camera2;

		private KeyBinding _camera1Bind;
		private KeyBinding _camera2Bind;

		public NodeGraphTest() : base("test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);

			_camera1Bind = _keyBindings.Register(new("camera1", Key.Number1));
			_camera2Bind = _keyBindings.Register(new("camera2", Key.Number2));
			
			this.SetupKeyBindings(_keyBindings);
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			_camera1 = new PerspectiveCamera(window) {
				FOV = 60,
				Position = new(0, 0, -3)
			};

			_camera2 = new OrthographicCamera(window) {
				FOV = 3,
				Position = new(2, 2, 2)
			};

			Camera = _camera1;

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

			if(_mesh != null) {
				AddChild(new Node3D() {
					Position = new(0, 0, 0),
					Model = new() {
						Meshes = new() { _mesh },
					}
				});
				
				AddChild(new Node3D() {
					Position = new(1, 2, 0),
					Model = new() {
						Meshes = new() { _mesh },
						Material = new() {
							Albedo = Color.FromArgb(200, 100, 50).ToVector4()
						}
					},
					InitialChildren = new() {
						["meow"] = new Node3D() {
							Position = new(0, 1, 0),
							Model = new() {
								Meshes = new() { _mesh },
								Material = new() {
									Albedo = Color.FromArgb(0, 100, 50).ToVector4()
								}
							},
							InitialChildren = new() {
								["bowwow"] = new Node3D() {
									Position = new(-1, 0, -3),
									Model = new() {
										Meshes = new() { _mesh },
										Material = new() {
											Albedo = Color.FromArgb(0, 50, 100).ToVector4()
										}
									}
								},
								["awoo"] = new Node3D() {
									Position = new(0, 1, 0),
									Model = new() {
										Meshes = new() { _mesh },
										Material = new() {
											Albedo = Color.FromArgb(100, 30, 100).ToVector4()
										}
									}
								}
							}
						}
					}
				});

				/*int wall = 16;
				
				for(int y = 0; y < (wall * 2); y += 2) {
					var n = new Node3D() {
						Position = new(0, y, 0),
						Model = new() {
							Material = new() {
								Color = new(RANDOM.NextSingle(), RANDOM.NextSingle(),
								            RANDOM.NextSingle(), 1)
							},
							Meshes = new[] { _mesh }
						}
					};
					
					for(int x = 0; x < (wall * 2); x += 2) {
						if(x == 0) continue;
						
						var n1 = new Node3D() {
							Position = new(x, y, 0),
							Model = new() {
								Material = new() {
									Color = new(RANDOM.NextSingle(), RANDOM.NextSingle(),
									            RANDOM.NextSingle(), 1)
								},
								Meshes = new[] { _mesh }
							}
						};
						
						for(int z = 0; z < (wall * 2); z += 2) {
							if(z == 0) continue;
							
							var n2 = new Node3D() {
								Position = new(x, y, z),
								Model = new() {
									Material = new() {
										Color = new(RANDOM.NextSingle(), RANDOM.NextSingle(),
										            RANDOM.NextSingle(), 1)
									},
									Meshes = new[] { _mesh }
								}
							};

							n1.Children.Add(n2);
						}
						
						n.Children.Add(n1);
					}
					
					Children.Add(n);
				}*/
			}
			
			_overlay = new(this);
			_overlay.Render += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Camera position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");

					if(Camera != null) {
						var fov = Camera.FOV;
						ImGui.SliderFloat("Camera FOV", ref fov, 1f, 179.9f);
						Camera.FOV = fov;
					}
					
					ImGui.Separator();
					int childrenTotal = 0;

					void DrawChildren(NodeBase node) {
						int i = 0;

						foreach(var child in node.Children.Values) {
							ImGui.PushID(i);
							if(ImGui.TreeNode("", $"{child.GetType().Name}: {child}")) {
								ImGui.Text($"Parent: {child.Parent}");
								ImGui.Separator();
								DrawChildren(child);
								ImGui.TreePop();
							}
							ImGui.PopID();
						}
					}
					
					DrawChildren(this);
					ImGui.Separator();
					ImGui.Text($"Children node count: {Children.Count}");
					ImGui.Text($"Children total: {childrenTotal}");
					
					ImGui.End();
				}
				
				//ImGui.ShowDemoWindow();
			};

			window.GetMice()[0].MouseMove += (_, pos) => {
				if(Camera != null) _freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			if(Camera != null) _freeCamera.Update(Camera, ref mouse, delta);

			if(_camera1Bind.Pressed) Camera = _camera1;
			if(_camera2Bind.Pressed) Camera = _camera2;
			
			_keyBindings.Update(Window.Input.Keyboards[0]);
		}
	}
}