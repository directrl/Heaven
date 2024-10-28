using System.Numerics;
using Coelum.Common.Input;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Components;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.Node;
using Coelum.Phoenix.Scene;
using Coelum.Phoenix.UI;
using Coelum.World;
using Coelum.World.Components;
using Flecs.NET.Bindings;
using Flecs.NET.Core;
using ImGuiNET;
using PhoenixPlayground.Prefabs;

namespace PhoenixPlayground.Scenes {
	
	public class WorldTest : Scene3D, IWorldContainer<Renderable, Tickable> {

		private static readonly Random RANDOM = new();
		
		public World World { get; init; }
		
		public Query<Renderable> RenderableQuery { get; }
		public Query<Renderable, Position3D, Rotation3D, Scale3D> Renderable2Query { get; }
		public Query<Tickable> TickableQuery { get; }

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private DebugUI _debug;

		public WorldTest() : base("world-test") {
			World = World.Create();
			World.Import<Ecs.Units>();
			World.Import<Ecs.Stats>();

			RenderableQuery = World.QueryBuilder<Renderable>().Build();
			Renderable2Query = World.QueryBuilder<Renderable, Position3D, Rotation3D, Scale3D>().Build();
			TickableQuery = World.QueryBuilder<Tickable>().Build();

			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			Camera = new PerspectiveCamera(window) {
				FOV = 60
			};

			_debug = new(this);
			_debug.AdditionalInfo += (_, _) => {
				if(ImGui.Begin("Camera")) {
					ImGui.Text($"position: {Camera?.Position.ToString() ?? "Unknown"}");
					ImGui.Text($"pitch: {Camera?.Pitch.ToString() ?? "Unknown"}");
					ImGui.Text($"yaw: {Camera?.Yaw.ToString() ?? "Unknown"}");
					ImGui.End();
				}
			};

			World.App().EnableRest();
			World.Set(default(flecs.EcsRest));
			
			TestEntity.AddPrefab(World);

			for(int i = 0; i < 1024; i++) {
				TestEntity.Create(World)
				          .Set<Scale3D>(new(RANDOM.NextSingle() + 0.5f, RANDOM.NextSingle() + 0.5f, RANDOM.NextSingle() + 0.5f))
				          .Set<Position3D>(new(RANDOM.Next(-64, 64), RANDOM.Next(-64, 64), RANDOM.Next(-64, 64)));
			}
			
			// AddChild(new Node3D() {
			// 	Model = TestEntity.MODEL
			// });

			window.GetMice()[0].MouseMove += (_, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);

			World.Progress(delta);
		}

		public override void OnRender(float delta) {
			base.OnRender(delta);
			
			Renderable2Query.Each((Entity e, ref Renderable renderable, ref Position3D p, ref Rotation3D r, ref Scale3D s) => {
				var scaleMatrix = Matrix4x4.CreateScale(s.X, s.Y, s.Z);
				var positionMatrix = Matrix4x4.CreateTranslation(p.X, p.Y, p.Z);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(r.Y, r.X, r.Z);
				
				var modelMatrix = scaleMatrix * positionMatrix * rotationMatrix;

				PrimaryShader.SetUniform("model", modelMatrix);
				renderable.Render.Invoke(delta, PrimaryShader);
			});
		}
	}
}