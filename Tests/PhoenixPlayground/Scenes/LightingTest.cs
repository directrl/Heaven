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

		private KeyBinding _lightXpos;
		private KeyBinding _lightXneg;
		private KeyBinding _lightYpos;
		private KeyBinding _lightYneg;
		private KeyBinding _lightZpos;
		private KeyBinding _lightZneg;
	#endregion
		
		private EcsSystem _testCubeMove;
		private EcsSystem _testCubeRotate;

		private Camera3D? _camera;
		private Node _lightCube;

		public LightingTest() : base("light-test") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);

			_phong = _keyBindings.Register(new("phong", Key.Number1));
			_gouraud = _keyBindings.Register(new("gouraud", Key.Number2));

			_lightXpos = _keyBindings.Register(new("x1", Key.U));
			_lightXneg = _keyBindings.Register(new("x2", Key.J));
			_lightYpos = _keyBindings.Register(new("y1", Key.I));
			_lightYneg = _keyBindings.Register(new("y2", Key.K));
			_lightZpos = _keyBindings.Register(new("z1", Key.O));
			_lightZneg = _keyBindings.Register(new("z2", Key.L));
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
			ShaderOverlays.AddRange(SceneEnvironment.OVERLAYS);
			ShaderOverlays.AddRange(PhongShading.OVERLAYS);
			//ShaderOverlays.AddRange(GouraudShading.OVERLAYS);

			_testCubeMove = new("cube move", (root, delta) => {
				root.Query<Transform, Light>()
				    .Each((node, t, l) => {
					    if(t is not Transform3D t3d) return;
					    if(!node.HasComponent<TestLightMove>()) return;

					    if(l is DirectionalLight) {
						    t3d.Rotation = new(
							    -MathF.PI,
							    MathF.Sin((float) Window.SilkImpl.Time) * (MathF.PI / 2),
							    0
							);
						    
						    Console.WriteLine(((DirectionalLight)l).Direction);
					    } else {
						    t3d.Position = new(
							    MathF.Sin((float) Window.SilkImpl.Time) * 6,
							    0,
							    MathF.Cos((float) Window.SilkImpl.Time) * 6
						    );
					    }
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
				AmbientLight = Color.DarkSlateGray
			});

			if(_camera == null) {
				_camera = new PerspectiveCamera(window) {
					FOV = 60,
					Current = true
				};
				_camera.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);
			}
			Add(_camera);

			{
				// var centerCube = new ModelNode(ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "crt.glb"]));
				// centerCube.AddComponent(new TestCubeRotate());
				
				var playgroundModel = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "playground.glb"]);
				var playground = new ModelNode(playgroundModel);
				playground.GetComponent<Transform, Transform3D>()
				          .Position = new(0, -0, -0);
				Add(playground);

				var crt = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "crt.glb"]);

				for(int i = 0; i < 4; i++) {
					Vector3 pos = new();

					int max = 10;
					if(i == 0) pos = new(0, 0, max);
					else if(i == 1) pos = new(0, 0, -max);
					else if(i == 2) pos = new(max, 0, 0);
					else pos = new(-max, 0, 0);
					
					var model = new ModelNode(crt);
					model.AddComponent(new TestCubeRotate());
					model.GetComponent<Transform, Transform3D>()
					     .Position = pos;
					
					Add(model);
				}

				_lightCube = new ColorCube(Color.AntiqueWhite) {
					Name = "light cube center"
				};
				_lightCube.AddComponent(new TestLightMove());
				_lightCube.AddComponent<Light>(new PointLight() {
					Distance = 64,
				});
				_lightCube.GetComponent<Transform, Transform3D>()
				          .Scale = new(0.5f);

				var lightCube2 = new ColorCube(Color.Bisque) {
					Name = "spot light cube"
				};
				lightCube2.AddComponent<Light>(new SpotLight() {
					Distance = 64
				});
				lightCube2.GetComponent<Transform, Transform3D>()
				          .Position = new(0.0f, 5.0f, -0f);
				lightCube2.GetComponent<Transform, Transform3D>()
				          .Rotation = new(0.0f, 1.0f, 0.0f);

				_lightCube = lightCube2;
				//Add(_lightCube);
				Add(lightCube2);
			}
			
			AddSystem("UpdatePre", _testCubeMove); // TODO phases should be enums or smth
			AddSystem("UpdatePre", _testCubeRotate);

			var debug = new DebugUI(this);
			debug.AdditionalInfo += (_, _) => {
				ImGui.Text($"Rot: {_lightCube.GetComponent<Transform, Transform3D>().GlobalRotation}");
				
				Query<Light>()
					.Each((node, l) => {
						if(l is not PointLight pl) return;
						
						int dist = pl.Distance;
						ImGui.SliderInt($"{node.Name}: Light distance", ref dist, 0, 1000);
						pl.Distance = dist;

						if(l is SpotLight sl) {
							float cutoff = sl.Cutoff;
							ImGui.SliderAngle($"{node.Name}: Cutoff angle", ref cutoff, 0, 180);
							sl.Cutoff = cutoff;
						}
					})
					.Execute();
			};

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

			float change = delta * 0.5f;
			if(_lightXpos.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.X += change;
			if(_lightXneg.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.X -= change;
			if(_lightYpos.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.Y += change;
			if(_lightYneg.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.Y -= change;
			if(_lightZpos.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.Z += change;
			if(_lightZneg.Down) _lightCube.GetComponent<Transform, Transform3D>().Rotation.Z -= change;
			
			_keyBindings.Update(new SilkKeyboard(Window.Input.Keyboards[0]));
		}
	}
}