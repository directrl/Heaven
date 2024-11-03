using System.Drawing;
using System.Numerics;
using Coelum.Common.Input;
using Coelum.ECS;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using ImGuiNET;
using PhoenixPlayground.Components;
using PhoenixPlayground.Nodes;
using Silk.NET.Input;

namespace PhoenixPlayground.Scenes {
	
	public class LightingTest : PhoenixScene {
		
		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

	#region Keybindings
		private KeyBinding _phong;
		private KeyBinding _gouraud;
	#endregion
		
		private EcsSystem _testCubeMove;
		private EcsSystem _testCubeRotate;

		public LightingTest() : base("light-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);

			_phong = _keyBindings.Register(new("phong", Key.Number1));
			_gouraud = _keyBindings.Register(new("gouraud", Key.Number2));
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
			ShaderOverlays.AddRange(SceneEnvironment.OVERLAYS);
			ShaderOverlays.AddRange(PhongShading.OVERLAYS);
			ShaderOverlays.AddRange(GouraudShading.OVERLAYS);

			_testCubeMove = new("cube move", (root, delta) => {
				root.Query<Transform, TestLightMove>()
				    .Each((node, t, _) => {
					    if(t is not Transform3D t3d) return;
					    
					    t3d.Position = new(
						    MathF.Sin((float) Window.SilkImpl.Time) * 3,
						    0,
						    MathF.Cos((float) Window.SilkImpl.Time) * 3
					    );
				    })
				    .Execute();
			});
			
			_testCubeRotate = new("cube rotate", (root, delta) => {
				root.Query<Transform, TestCubeRotate>()
				    .Each((node, t, _) => {
					    if(t is not Transform3D t3d) return;

					    float rot = delta * 0.6f;
					    t3d.Rotation -= new Vector3(rot, rot, -rot);
				    })
				    .Execute();
			});
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			Add(new SceneEnvironment() {
				AmbientLight = Color.FromArgb(64, 64, 64)
			});

			var camera = new PerspectiveCamera(window) {
				FOV = 60,
				Current = true
			};
			camera.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);
			Add(camera);

			{
				var centerCube = new ModelNode(ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "crt.glb"]));
				centerCube.AddComponent(new TestCubeRotate());

				var lightCube = new ColorCube(Color.AntiqueWhite);
				lightCube.AddComponent(new TestLightMove());
				lightCube.AddComponent(new Light());
				lightCube.GetComponent<Transform, Transform3D>()
				         .Scale = new(0.5f);
				
				Add(centerCube);
				Add(lightCube);
			}
			
			AddSystem("UpdatePre", _testCubeMove); // TODO phases should be enums or smth
			AddSystem("UpdatePre", _testCubeRotate);

			_ = new DebugUI(this);

			window.GetMice()[0].MouseMove += (_, pos) => {
				if(CurrentCamera is Camera3D c3d) _freeCamera.CameraMove(c3d, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			if(CurrentCamera is Camera3D c3d) _freeCamera.Update(c3d, ref mouse, delta);

			if(_phong.Pressed) {
				Playground.AppLogger.Information("Switching to Phong shading");
				PrimaryShader.DisableOverlays(GouraudShading.OVERLAYS);
				PrimaryShader.EnableOverlays(PhongShading.OVERLAYS);
			}
			
			if(_gouraud.Pressed) {
				Playground.AppLogger.Information("Switching to Gouraud shading");
				PrimaryShader.EnableOverlays(GouraudShading.OVERLAYS);
				PrimaryShader.DisableOverlays(PhongShading.OVERLAYS);
			}
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}