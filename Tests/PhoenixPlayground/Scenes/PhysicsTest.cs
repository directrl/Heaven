using System.Drawing;
using BepuPhysics;
using BepuUtilities;
using Coelum.Common.Input;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.Callbacks;
using Coelum.Phoenix.Physics.ECS.Systems;
using Coelum.Phoenix.UI;
using PhoenixPlayground.Nodes.Physics;

namespace PhoenixPlayground.Scenes {
	
	public class PhysicsTest : PhoenixScene {

		private static readonly Random _RANDOM = new();
		
		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;
		private Camera3D _camera;
		
		private Simulation _simulation;
		private ThreadDispatcher _simulationDispatcher;
		
		public PhysicsTest() : base("physics") {
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays = new[] {
				Material.OVERLAYS,
				//SceneEnvironment.OVERLAYS,
				//PhongShading.OVERLAYS
			};
			
			(var s, var d) = this.CreatePhysicsSimulation<DefaultNarrowPhase, DefaultPoseIntegrator>();
			_simulation = s;
			_simulationDispatcher = d;
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			//AddSystem("FixedUpdate", new PhysicsObjectCreateSystem(_simulation));
			AddSystem("FixedUpdate", new PhysicsBodyUpdateSystem(_simulation));
			AddSystem("FixedUpdate", new PhysicsUpdateSystem(_simulation, _simulationDispatcher));
			AddSystem("RenderPost", new DebugPhysicsRenderSystem(PrimaryShader));

			_camera = Add(new PerspectiveCamera() {
				FOV = 60
			});
			Add(new Viewport(_camera, window.Framebuffer));
			
			/*Add(new SceneEnvironment() {
				AmbientColor = Color.FromArgb(16, 16, 16)
			});*/
			
			var plane = Add(new Plane(_simulation));
			plane.GetComponent<Transform3D>().Position = new(0, -5, 0);
			plane.GetComponent<Transform3D>().Scale = new(10, 1, 10);
			var model = plane.GetComponent<ModelRenderable>().Model;
			model.Materials[0].Albedo = Color.Brown.ToVector4();
			
			AddSystem("FixedUpdate", new("resize plane", (_, delta) => {
				var t3d = plane.GetComponent<Transform3D>();

				const int minSize = 10;
				float mult = 2;
				
				t3d.Scale.X = (float) Math.Sin(window.SilkImpl.Time) * mult + minSize;
				t3d.Scale.Z = (float) Math.Sin(window.SilkImpl.Time) * mult + minSize;
			}));

			int fumoCounter = 0;
			int fumoTimer = 0;
			AddSystem("FixedUpdate", new("fumo spawner", (_, delta) => {
				fumoTimer++;

				const int range = 10;

				if(fumoTimer >= 20) {
					var fumo = new Fumo(_simulation);
					fumo.GetComponent<Transform3D>().Position =
						new(
							_RANDOM.NextSingle() * range - (range / 2f),
							5,
							_RANDOM.NextSingle() * range - (range / 2f)
						);
					Add(fumo);
					
					fumoCounter++;
					fumoTimer = 0;
				}
				
				QueryChildren<Fumo>()
					.Each(fumo => {
						var t3d = fumo.GetComponent<Transform3D>();

						if(t3d.GlobalPosition.Y < -50) {
							// kill fumo :(
							RunLater(() => Remove(fumo));
						}
					})
					.Execute();
			}));

			UIOverlays.Add(new DebugOverlay(this));
			
			window.GetMice()[0].MouseMove += (_, pos) => {
				_freeCamera.CameraMove(_camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window!.GetMice()[0];
			_freeCamera.Update(_camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}

		public override void OnFixedUpdate(float delta) {
			base.OnFixedUpdate(delta);
			
			//_simulation.Timestep(delta, _simulationDispatcher);
		}
	}
}