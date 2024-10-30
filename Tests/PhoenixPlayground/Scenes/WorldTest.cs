using System.Numerics;
using Coelum.Common.Input;
using Coelum.Phoenix;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Components;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.Node;
using Coelum.Phoenix.Scene;
using Coelum.Phoenix.UI;
using Coelum.Common.Ecs;
using Coelum.Common.Ecs.Component;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.Ecs;
using Coelum.Phoenix.Ecs.Component;
using Coelum.Phoenix.Ecs.System;
using Flecs.NET.Bindings;
using Flecs.NET.Core;
using ImGuiNET;
using PhoenixPlayground.Prefabs;

namespace PhoenixPlayground.Scenes {
	
	public class WorldTest : Scene3D {

		private static readonly Random RANDOM = new();
		
		public World World { get; }
		public PrefabManager PrefabManager { get; }

		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private DebugUI _debug;

		public WorldTest() : base("world-test") {
			World = this.CreateWorld();
			PrefabManager = new(World);
			
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);
			
			ShaderOverlays.AddRange(Material.OVERLAYS);
		}

		private List<Entity> _t = new();

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			PrefabManager.Add<TestEntity>();

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

			for(int i = 0; i < 0 * 1024; i++) {
				_t.Add(PrefabManager.Create<TestEntity>()
				             .Set<Transform>(
					             new Transform3D(
						             rotation: new(RANDOM.NextSingle(),
		                                           RANDOM.NextSingle(),
		                                           RANDOM.NextSingle()), 
						             scale: new(RANDOM.NextSingle() + 0.5f,
						                        RANDOM.NextSingle() + 0.5f,
						                        RANDOM.NextSingle() + 0.5f),
		                             position: new(RANDOM.Next(-128, 128), 
		                                           RANDOM.Next(-128, 128),
		                                           RANDOM.Next(-128, 128)))));
			}

			var e1 = PrefabManager.Create<TestEntity>()
			                      .SetName("meow")
			                      .Set<Transform>(
				                      new Transform3D(
					                      position: new(-1, -1, -1)));
			var e2 = PrefabManager.Create<TestEntity>()
			                      .SetName("2")
			                      .Set<Transform>(
				                      new Transform3D(
					                      position: new(0, 2, 0)));
			var e3 = PrefabManager.Create<TestEntity>()
			                      .SetName("3")
			                      .Set<Transform>(
				                      new Transform3D(
					                      position: new(1, 0, 0)));
			var e4 = PrefabManager.Create<TestEntity>()
			                      .SetName("4")
			                      .Set<Transform>(
				                      new Transform3D(
					                      position: new(3, 0, -2),
					                      scale: new(1.5f, 1.5f, 1.5f)));
			var e5 = PrefabManager.Create<TestEntity>()
			                      .SetName("inner last")
			                      .Set<Transform>(
				                      new Transform3D(
					                      position: new(0, 1, 0)));

			e5.ChildOf(e4);
			e4.ChildOf(e3);
			e3.ChildOf(e2);
			e2.ChildOf(e1);

			// World.System<Transform>("test transform3d move")
			//      .Kind(Ecs.OnUpdate)
			//      .Each((Iter it, int i, ref Transform t) => {
			// 	     var t3d = (Transform3D) t;
			// 	     t3d.Position += new Vector3(
			// 		     RANDOM.Next(-5, 5) * it.DeltaTime(),
			// 		     RANDOM.Next(-5, 5) * it.DeltaTime(),
			// 		     RANDOM.Next(-5, 5) * it.DeltaTime()
			// 		 );
			//      });

			window.GetMice()[0].MouseMove += (_, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			// foreach(var e in _t) {
			// 	var t3d = (Transform3D) e.Get<Transform>();
			// 	t3d.Position.X += RANDOM.Next(-5, 5) * delta;
			// 	t3d.Position.Y += RANDOM.Next(-5, 5) * delta;
			// 	t3d.Position.Z += RANDOM.Next(-5, 5) * delta;
			// 	t3d.Dirty = true;
			// }

			var mouse = Window.GetMice()[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}
	}
}