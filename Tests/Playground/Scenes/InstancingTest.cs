using System.Diagnostics;
using System.Numerics;
using Coelum.Debug;
using Coelum.LanguageExtensions;
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
	
	public class InstancingTest : Scene3D {

		private static readonly Random RANDOM = new();
		
		private DebugUI _overlay;
		
		private Mesh? _mesh;
		private InstancedNode<Node3D>? _instObject;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private bool _instancing = true;
		
		List<Node3D> objects = new();

		public InstancingTest() : base("instancing") {
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
				int wall = 32;

				_instObject = new(new(_mesh), (int) Math.Pow(wall, 3));

				var sw = Stopwatch.StartNew();
				
				for(int y = 0; y < (wall * 2); y += 2)
				for(int x = 0; x < (wall * 2); x += 2)
				for(int z = 0; z < (wall * 2); z += 2) {
					var o1 = new Node3D() {
						Position = new(x, y, z),
						Model = new() {
							Material = new() {
								Albedo = new(RANDOM.NextSingle(), RANDOM.NextSingle(), RANDOM.NextSingle(), 1)
							}
						}
					};

					var o2 = new Node3D() {
						Position = o1.Position,
						Model = new() {
							Material = o1.Model.Material,
							Meshes = new() { _mesh }
						}
					};
					
					_instObject.Add(o1);
					objects.Add(o2);
				}
				
				sw.Stop();
				
				Playground.AppLogger.Information($"Created objects: {_instObject.NodeCount}" +
					$" in {sw.ElapsedMilliseconds}ms");
				
				Playground.AppLogger.Information("Building instances");
				sw.Restart();
				_instObject.Build();
				sw.Stop();
				Playground.AppLogger.Information($"Done in {sw.ElapsedMilliseconds}ms");
			}
			
			_overlay = new(this);
			_overlay.AdditionalInfo += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Camera position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");
					ImGui.Text($"Object count: {_instObject?.NodeCount.ToString() ?? "Unknown"}");
					ImGui.Text($"Using instancing: {_instancing}");
					ImGui.Checkbox("Use instancing", ref _instancing);
					ImGui.End();
				}
			};

			window.GetMice()[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.Input.Mice[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);

			if(_instancing) {
				_instObject?.Load(PrimaryShader);
				_instObject?.Render(PrimaryShader);
			} else {
				foreach(var o in objects) {
					o.Load(PrimaryShader);
					o.Render();
				}
			}
		}
	}
}