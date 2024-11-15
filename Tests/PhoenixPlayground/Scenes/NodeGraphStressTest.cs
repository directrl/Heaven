using System.Drawing;
using System.Numerics;
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
	
	public class NodeGraphStressTest : PhoenixScene {
		
		private static readonly Random RANDOM = new();

		private Camera3D _camera;
		private Node _player;
		
		private Mesh? _mesh;

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

	#region Keybindings
		private KeyBinding _playerForward;
		private KeyBinding _playerBackward;
		private KeyBinding _playerLeft;
		private KeyBinding _playerRight;
		
		private KeyBinding _playerRot1;
		private KeyBinding _playerRot2;
		private KeyBinding _playerRot3;
	#endregion

		private float _moved = 0;
		private EcsSystem _moveStressTest;

		public NodeGraphStressTest() : base("node-graph-stresstest") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);

		#region Keybindings
			_playerForward = _keyBindings.Register(new("pforward", Key.I));
			_playerBackward = _keyBindings.Register(new("pback", Key.K));
			_playerLeft = _keyBindings.Register(new("pleft", Key.J));
			_playerRight = _keyBindings.Register(new("pright", Key.L));
			
			_playerRot1 = _keyBindings.Register(new("prot1", Key.V));
			_playerRot2 = _keyBindings.Register(new("prot2", Key.B));
			_playerRot3 = _keyBindings.Register(new("prot3", Key.N));
		#endregion
			
			this.SetupKeyBindings(_keyBindings);

			ShaderOverlays = new[] {
				Material.OVERLAYS
			};

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

			_player = new TestNode() {
				Name = "player"
			};
			_player.GetComponent<Transform, Transform3D>().Position = new(0, -3, -3);
			Add(_player);
			
			_camera = new PerspectiveCamera() {
				FOV = 60
			};
			_camera.GetComponent<Transform, Transform3D>().Position = new(0, 0, -5);
			_player.Add(_camera);
			
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
			
			AddSystem("UpdatePre", _moveStressTest);

			var debugOverlay = new DebugOverlay(this);
			debugOverlay.AdditionalInfo += (delta) => {
				ImGui.Separator();
				
				/*if(ImGui.Begin("Info"))*/ {
					ImGui.Text($"Camera position: {_camera.GetComponent<Transform, Transform3D>().Position.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera pitch: {_camera.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"Camera yaw: {_camera.Yaw.ToString() ?? "Unknown"}");

					var fov = _camera.FOV;
					ImGui.SliderFloat("Camera FOV", ref fov, 1f, 179.9f);
					_camera.FOV = fov;
					
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
					ImGui.End();
				}
			};
			
			UIOverlays.Add(debugOverlay);

			window.GetMice()[0].MouseMove += (_, pos) => {
				_freeCamera.CameraMove(_camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			_freeCamera.Update(_camera, ref mouse, delta);

			float playerMove = 10f * delta;
			if(_playerForward.Down) _player.GetComponent<Transform, Transform3D>().Position.Z += playerMove;
			if(_playerBackward.Down) _player.GetComponent<Transform, Transform3D>().Position.Z -= playerMove;
			if(_playerLeft.Down) _player.GetComponent<Transform, Transform3D>().Position.X += playerMove;
			if(_playerRight.Down) _player.GetComponent<Transform, Transform3D>().Position.X -= playerMove;

			float playerRot = 1f * delta;
			if(_playerRot1.Down) _player.GetComponent<Transform, Transform3D>().Rotation.X += playerRot;
			if(_playerRot2.Down) _player.GetComponent<Transform, Transform3D>().Rotation.Y += playerRot;
			if(_playerRot3.Down) _player.GetComponent<Transform, Transform3D>().Rotation.Z += playerRot;
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}