using System.Drawing;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Texture;
using Coelum.Common.Input;
using Coelum.LanguageExtensions;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using ImGuiNET;
using PhoenixPlayground.Nodes;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace PhoenixPlayground.Scenes {
	
	public class NodeGraphTest : PhoenixScene {
		
		private static readonly Random RANDOM = new();
		
		private DebugUI _overlay;
		
		private Mesh? _mesh;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private Camera3D _camera1;
		private Camera3D _camera2;

		private KeyBinding _camera1Bind;
		private KeyBinding _camera2Bind;

		public NodeGraphTest() : base("node-graph-stresstest") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);

			_camera1Bind = _keyBindings.Register(new("camera1", Key.Number1));
			_camera2Bind = _keyBindings.Register(new("camera2", Key.Number2));
			
			this.SetupKeyBindings(_keyBindings);
			
			PrimaryShader.AddOverlays(Material.OVERLAYS);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			_camera1 = new PerspectiveCamera() {
				FOV = 60,
				Current = true
			};
			_camera1.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);
			Add(_camera1);

			_camera2 = new OrthographicCamera() {
				FOV = 3
			};
			_camera2.GetComponent<Transform, Transform3D>().Position = new(2, 2, 2);
			Add(_camera2);

			{
				Add(new ColorCube(Color.AntiqueWhite));

				var c1 = new ColorCube(Color.Chocolate) {
					Name = "meow"
				};
				c1.GetComponent<Transform, Transform3D>().Position = new(1, 2, 2);

				var c3 = new ColorCube(Color.ForestGreen) {
					Name = "awoo",
					Children = new[] { c1 }
				};
				c3.GetComponent<Transform, Transform3D>().Position = new(-1, 0, -3);

				var c4 = new ColorCube(Color.DimGray) {
					Name = "rawr"
				};
				c4.GetComponent<Transform, Transform3D>().Position = new(0, 2, 0);
				
				var c2 = new ColorCube(Color.Fuchsia) {
					Name = "bowwow",
					Children = new[] {
						c3, c4
					}
				};
				c2.GetComponent<Transform, Transform3D>().Position = new(0, 1, 0);

				// c3.Add(c1);
				// c2.Add(c3, c4);

				Add(c2);
				//c3.Add(c1);
			}

			_overlay = new DebugUI(this);
			_overlay.AdditionalInfo += (delta, args) => {
				ImGui.Separator();

				var camera = (Camera3D) CurrentCamera;
				
				/*if(ImGui.Begin("Info"))*/ {
					ImGui.Text($"Camera position: {camera.GetComponent<Transform, Transform3D>().Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {camera.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {camera.Yaw.ToString() ?? "Unknown"}");

					var fov = camera.FOV;
					ImGui.SliderFloat("Camera FOV", ref fov, 1f, 179.9f);
					camera.FOV = fov;
					
					ImGui.Separator();
					int childCount = 0;
					
					QueryChildren()
						.Each(node => {
							childCount++;
							ImGui.Text($"{node.Path}: {node}");
						})
						.Execute();
					
					ImGui.Separator();
					ImGui.Text($"Child count: {childCount}");
					ImGui.End();
				}
			};

			window.GetMice()[0].MouseMove += (_, pos) => {
				var camera = CurrentCamera;
				if(camera is Camera3D c3d) _freeCamera.CameraMove(c3d, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			var camera = CurrentCamera;
			if(camera is Camera3D c3d) _freeCamera.Update(c3d, ref mouse, delta);

			if(_camera1Bind.Pressed) _camera1.Current = true;
			if(_camera2Bind.Pressed) _camera2.Current = true;
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}