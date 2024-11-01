using System.Drawing;
using System.Numerics;
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
	
	public class NodeGraphStressTest : Scene3D {
		
		private static readonly Random RANDOM = new();
		
		private DebugUI _overlay;
		
		private Mesh? _mesh;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private float _moved = 0;
		private EcsSystem _moveStressTest;

		public NodeGraphStressTest() : base("node-graph-stresstest") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);

			_moveStressTest = new("move test", (root, delta) => {
				root.Query<Transform>()
				    .Parallel(true)
				    .Each((node, t) => {
					    if(t is not Transform3D t3d) return;

					    if(node.Name == "rootChild") {
						    t3d.Position = new(
							    MathF.Sin((float) Window.SilkImpl.Time) * 5,
							    0,
							    MathF.Cos((float) Window.SilkImpl.Time) * 5
							);

						    t3d.Rotation += new Vector3(delta / 4, -delta / 4, delta / 4);
					    } else {
						    //t3d.Rotation += new Vector3(delta / 2, delta / 2, -delta / 2);
					    }
				    })
				    .Execute();
			}) {
				Enabled = false
			};
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			Camera = new PerspectiveCamera(window) {
				FOV = 60
			};
			Camera.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);

			{
				var size = 24;
				var rootChild = new TestNode() {
					Root = this,
					Name = "rootChild"
				};

				for(int y = 8; y < size + 8; y++) {
					for(int x = -size / 2; x < size / 2; x++) {
						for(int z = -size / 2; z < size / 2; z++) {
							var n = new ColorCube(new Color().Randomize());
							
							n.GetComponent<Transform, Transform3D>().Position = new(
								x,
								y,
								z
							);
							
							rootChild.Add(n);
						}
					}
				}
				
				Add(rootChild);
			}
			
			System("UpdatePre", _moveStressTest);

			_overlay = new DebugUI(this);
			_overlay.AdditionalInfo += (delta, args) => {
				ImGui.Separator();
				
				/*if(ImGui.Begin("Info"))*/ {
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
							//ImGui.Text($"{node.Path}: {node}");
						})
						.Execute();
					
					ImGui.Separator();
					ImGui.Text($"Child count: {childCount}");
					
					ImGui.Separator();
					ImGui.Checkbox("Move test", ref _moveStressTest.Enabled);
					
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
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}