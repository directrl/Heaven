using System.Drawing;
using BepuPhysics;
using BepuUtilities;
using Coelum.Common.Input;
using Coelum.Core;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.Callbacks;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Systems;
using Coelum.Phoenix.UI;
using Coelum.Resources;
using PhoenixPlayground.Nodes.Physics;
using Silk.NET.Input;

namespace PhoenixPlayground.Scenes {
	
	public class PhysicsTest : PhoenixScene {

		private static readonly Random _RANDOM = new();
		
		private FreeCamera _freeCamera;
		
		private Camera3D _camera;
		private Player _player;
		
		private Simulation _simulation;
		private ThreadDispatcher _simulationDispatcher;

	#region Key bindings
		private KeyBinding _playerForward;
		private KeyBinding _playerBackward;
		private KeyBinding _playerLeft;
		private KeyBinding _playerRight;
		private KeyBinding _playerJump;
	#endregion
		
		public PhysicsTest() : base("physics") {
			_freeCamera = new(KeyBindings);

			_playerForward = KeyBindings.Register(new("pf", Key.W));
			_playerBackward = KeyBindings.Register(new("pb", Key.S));
			_playerLeft = KeyBindings.Register(new("pl", Key.A));
			_playerRight = KeyBindings.Register(new("pr", Key.D));
			_playerJump = KeyBindings.Register(new("pj", Key.Space));
			
			ShaderOverlays.AddRange(new[] {
				Material.OVERLAYS,
				//SceneEnvironment.OVERLAYS,
				//PhongShading.OVERLAYS
			});
			
			(var s, var d) = this.CreatePhysicsSimulation<DefaultNarrowPhase, DefaultPoseIntegrator>();
			_simulation = s;
			_simulationDispatcher = d;
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			ClearColor = Color.DimGray;
			ModelLoader.Load(Heaven.AppResources[ResourceType.MODEL, "crt.glb"]);
			
			//AddSystem("FixedUpdate", new PhysicsObjectCreateSystem(_simulation));
			AddSystem(new PhysicsBodyUpdateSystem(_simulation));
			AddSystem(new PhysicsUpdateSystem(_simulation, _simulationDispatcher));
			AddSystem(new DebugPhysicsRenderSystem(PrimaryShader));

			_player = Add(new Player(_simulation));
			_player.GetComponent<Transform3D>().Scale = new(0.5f, 0.5f, 0.5f);
			
			_camera = _player.Add(new PerspectiveCamera() {
				FOV = 60
			});
			_camera.GetComponent<Transform3D>().Position = new(-5, 5, -5);
			
			Add(new Viewport(_camera, window.Framebuffer));
			
			/*Add(new SceneEnvironment() {
				AmbientColor = Color.FromArgb(16, 16, 16)
			});*/
			
			var plane = Add(new Plane(_simulation));
			plane.GetComponent<Transform3D>().Position = new(0, -5, 0);
			plane.GetComponent<Transform3D>().Scale = new(10, 1, 10);
			var model = plane.GetComponent<ModelRenderable>().Model;
			model.Materials[0].Albedo = Color.Crimson.ToVector4();
			
			AddSystem(new("resize plane", SystemPhase.FIXED_UPDATE, (_, delta) => {
				var t3d = plane.GetComponent<Transform3D>();

				const int minSize = 10;
				float mult = 2;
				
				t3d.Scale.X = (float) Math.Sin(window.SilkImpl.Time) * mult + minSize;
				t3d.Scale.Z = (float) Math.Sin(window.SilkImpl.Time) * mult + minSize;

				plane.GetPhysicsComponent().Dirty = true;
			}));

			int fumoCounter = 0;
			int fumoTimer = 0;
			
			AddSystem(new("fumo spawner", SystemPhase.FIXED_UPDATE, (_, delta) => {
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
			_freeCamera.Update(_camera, ref mouse, delta, noMove: true);

			_player.GetBody(out var playerBody);

			float playerSpeed = 10 * delta;
			float playerJumpStrength = 8;
			
			if(_playerForward.Down) {
				playerBody.Velocity.Linear.Z += playerSpeed;
				playerBody.Awake = true;
			}

			if(_playerBackward.Down) {
				playerBody.Velocity.Linear.Z -= playerSpeed;
				playerBody.Awake = true;
			}

			if(_playerLeft.Down) {
				playerBody.Velocity.Linear.X -= playerSpeed;
				playerBody.Awake = true;
			}

			if(_playerRight.Down) {
				playerBody.Velocity.Linear.X += playerSpeed;
				playerBody.Awake = true;
			}

			if(_playerJump.Pressed) {
				playerBody.Velocity.Linear.Y += playerJumpStrength;
				playerBody.Awake = true;
			}
			
			UpdateKeyBindings();
		}
	}
}