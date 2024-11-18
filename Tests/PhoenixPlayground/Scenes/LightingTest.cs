using System.Drawing;
using System.Numerics;
using Coelum.Common.Input;
using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.LanguageExtensions;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ECS.Nodes;
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
		private Node _controlledLight;

		private bool _doGouraud = false;

		public LightingTest() : base("light-test") {
			_freeCamera = new(KeyBindings);

			_phong = KeyBindings.Register(new("phong", Key.Number1));
			_gouraud = KeyBindings.Register(new("gouraud", Key.Number2));

			_lightXpos = KeyBindings.Register(new("x1", Key.U));
			_lightXneg = KeyBindings.Register(new("x2", Key.J));
			_lightYpos = KeyBindings.Register(new("y1", Key.I));
			_lightYneg = KeyBindings.Register(new("y2", Key.K));
			_lightZpos = KeyBindings.Register(new("z1", Key.O));
			_lightZneg = KeyBindings.Register(new("z2", Key.L));

			ShaderOverlays.AddRange(new[] {
				Material.OVERLAYS,
				SceneEnvironment.OVERLAYS,
				PhongShading.OVERLAYS,
				GouraudShading.OVERLAYS
			});

			_testCubeMove = new("cube move", SystemPhase.UPDATE, (root, delta) => {
				root.Query<Transform, Light>()
				    .Each((node, t, l) => {
					    if(t is not Transform3D t3d) return;
					    if(!node.HasComponent<TestLightMove>()) return;

					    if(node.Name == "orbit") {
						    t3d.Position = new(
							    MathF.Cos((float) Window.SilkImpl.Time) * 23,
							    MathF.Sin((float) Window.SilkImpl.Time) * 10,
							    MathF.Cos((float) Window.SilkImpl.Time) * 23
							);
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
			
			/*_testCubeRotate = new("cube rotate", (root, delta) => {
				root.Query<Transform, TestCubeRotate>()
				    .Each((node, t, _) => {
					    if(t is not Transform3D t3d) return;

					    float rot = delta * 0.6f;
					    t3d.Rotation -= new Vector3(rot, rot, -rot);
				    })
				    .Execute();
			});*/
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			Add(new SceneEnvironment() {
				AmbientColor = Color.FromArgb(6, 6, 6)
			});

			if(_camera == null) {
				_camera = new PerspectiveCamera() {
					FOV = 60
				};
				_camera.GetComponent<Transform, Transform3D>().Position = new(0, 0, -3);
			}
			Add(_camera);
			Add(new Viewport(_camera, window.Framebuffer));

			{
				var playgroundModel =
					ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "playground.glb"]);
				var playground = new ModelNode(playgroundModel);
				playground.GetComponent<Transform, Transform3D>()
				          .Position = new(0, -5, -0);
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

				var light1 = new ColorCube(Color.HotPink);
				light1.AddComponent<Light>(new PointLight() {
					Diffuse = Color.HotPink,
					Specular = Color.HotPink
				});
				light1.AddComponent(new TestLightMove());
				Add(light1);

				var light2 =
					new ModelNode(
						ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "light.glb"]));
				light2.AddComponent<Light>(new SpotLight() {
					Distance = 100
				});
				light2.GetComponent<Transform, Transform3D>()
				      .Position = new(0.0f, 5.0f, -0f);
				light2.GetComponent<Transform, Transform3D>()
				      .Rotation.X = -45.0f.ToRadians();

				Add(light2);
				_controlledLight = light2;

				var light3 = new ColorCube(Color.Yellow) {
					Name = "orbit"
				};
				light3.AddComponent<Light>(new PointLight() {
					Diffuse = Color.Yellow,
					Specular = Color.Yellow
				});
				light3.AddComponent(new TestLightMove());
				Add(light3);

				var light4 = new Node();
				light4.AddComponent<Light>(new DirectionalLight() {
					Diffuse = Color.FromArgb(1, 1, 1)
				});
				light4.AddComponent<Transform>(new Transform3D());
				light4.GetComponent<Transform, Transform3D>()
				      .Pitch = 70;
				light4.GetComponent<Transform, Transform3D>()
				      .Yaw = 15;
				Add(light4);
			}
			
			AddSystem(SystemPhase.UPDATE_PRE, _testCubeMove); // TODO phases should be enums or smth
			//AddSystem("UpdatePre", _testCubeRotate);
			
			AddQuery(new ComponentQuery<TestCubeRotate, Transform>(SystemPhase.UPDATE_PRE, (_, t) => {
				if(t is not Transform3D t3d) return;
					
				float rot = (1/60f) * 0.6f;
				t3d.Rotation -= new Vector3(rot, rot, -rot);
			}));

			var debugOverlay = new DebugOverlay(this);
			debugOverlay.AdditionalInfo += _ => {
				if(PrimaryCamera is Camera3D c3d) {
					ImGui.Text($"Camera pos: {c3d.GetComponent<Transform, Transform3D>().GlobalPosition}");
					ImGui.Text($"Camera yaw: {c3d.Yaw}");
					ImGui.Text($"Camera pitch: {c3d.Pitch}");
					ImGui.Separator();
				}
				
				// ImGui.Text($"Rot: {_lightCube.GetComponent<Transform, Transform3D>().GlobalRotation}");
				// ImGui.Text($"Yaw: {_lightCube.GetComponent<Transform, Transform3D>().GlobalYaw.ToDegrees()}");
				// ImGui.Text($"Pitch: {_lightCube.GetComponent<Transform, Transform3D>().GlobalPitch.ToDegrees()}");
				// ImGui.Text($"Roll: {_lightCube.GetComponent<Transform, Transform3D>().GlobalRoll.ToDegrees()}");
				
				Query<Light>()
					.Each((node, l) => {
						if(l is not PointLight pl) return;
						
						int dist = pl.Distance;
						ImGui.SliderInt($"{node.Name}: Light distance", ref dist, 0, 1000);
						pl.Distance = dist;

						if(l is SpotLight sl) {
							float cutoff = sl.Cutoff;
							ImGui.SliderAngle($"{node.Name}: Cutoff angle", ref cutoff, 0, 90);
							sl.Cutoff = cutoff;
							
							float ocutoff = sl.Fade;
							ImGui.SliderAngle($"{node.Name}: Fade angle", ref ocutoff, 0, 10);
							sl.Fade = ocutoff;
						}
					})
					.Execute();
			};
			
			UIOverlays.Add(debugOverlay);

			window.GetMice()[0].MouseMove += (_, pos) => {
				if(PrimaryCamera is Camera3D c3d) _freeCamera.CameraMove(c3d, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			if(PrimaryCamera is Camera3D c3d) _freeCamera.Update(c3d, ref mouse, delta);

			if(_phong.Pressed) {
				Playground.AppLogger.Information("Switching to Phong shading");
				_doGouraud = false;
			}
			
			if(_gouraud.Pressed) {
				Playground.AppLogger.Information("Switching to Gouraud shading");
				_doGouraud = true;
			}

			float change = delta * 0.5f;
			if(_lightXpos.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.X += change;
			if(_lightXneg.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.X -= change;
			if(_lightYpos.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.Y += change;
			if(_lightYneg.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.Y -= change;
			if(_lightZpos.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.Z += change;
			if(_lightZneg.Down) _controlledLight.GetComponent<Transform, Transform3D>().Rotation.Z -= change;
			
			UpdateKeyBindings();

			PrimaryShader.Bind();
			if(_doGouraud) {
				PrimaryShader.EnableOverlays(GouraudShading.OVERLAYS);
				PrimaryShader.DisableOverlays(PhongShading.OVERLAYS);
			} else {
				PrimaryShader.DisableOverlays(GouraudShading.OVERLAYS);
				PrimaryShader.EnableOverlays(PhongShading.OVERLAYS);
			}
		}
	}
}