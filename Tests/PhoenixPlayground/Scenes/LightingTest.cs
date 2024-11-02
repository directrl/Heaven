using System.Drawing;
using Coelum.Common.Input;
using Coelum.ECS;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using ImGuiNET;
using PhoenixPlayground.Components;
using PhoenixPlayground.Nodes;
using Silk.NET.Input;

namespace PhoenixPlayground.Scenes {
	
	public class LightingTest : PhoenixScene {
		
		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private EcsSystem _testCubeMove;

		public LightingTest() : base("light-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
			ShaderOverlays.AddRange(SceneEnvironment.OVERLAYS);

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
				var centerCube = new ColorCube(Color.IndianRed);

				var lightCube = new ColorCube(Color.AntiqueWhite);
				lightCube.AddComponent(new TestLightMove());
				lightCube.GetComponent<Transform, Transform3D>()
				         .Scale = new(0.5f);
				
				Add(centerCube);
				centerCube.Add(lightCube);
			}
			
			AddSystem("UpdatePre", _testCubeMove); // TODO phases should be enums or smth

			_ = new DebugUI(this);

			window.GetMice()[0].MouseMove += (_, pos) => {
				if(CurrentCamera is Camera3D c3d) _freeCamera.CameraMove(c3d, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			if(CurrentCamera is Camera3D c3d) _freeCamera.Update(c3d, ref mouse, delta);
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}