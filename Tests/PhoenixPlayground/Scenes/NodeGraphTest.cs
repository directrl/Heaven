using System.Drawing;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Scene;
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
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			_camera1 = new PerspectiveCamera(window) {
				FOV = 60
			};
			_camera1.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);

			_camera2 = new OrthographicCamera(window) {
				FOV = 3
			};
			_camera2.GetComponent<Transform, Transform3D>().Position = new(2, 2, 2);

			Camera = _camera1;

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

			_overlay = new(this);
			_overlay.Render += (delta, args) => {
				if(ImGui.Begin("Info")) {
					ImGui.Text($"Camera position: {Camera?.GetComponent<Transform, Transform3D>().Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");

					if(Camera != null) {
						var fov = Camera.FOV;
						ImGui.SliderFloat("Camera FOV", ref fov, 1f, 179.9f);
						Camera.FOV = fov;
					}
					
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
				if(Camera != null) _freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			if(Camera != null) _freeCamera.Update(Camera, ref mouse, delta);

			if(_camera1Bind.Pressed) Camera = _camera1;
			if(_camera2Bind.Pressed) Camera = _camera2;
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}