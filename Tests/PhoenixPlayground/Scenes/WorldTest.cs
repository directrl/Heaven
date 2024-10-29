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

			for(int i = 0; i < 32 * 1024; i++) {
				PrefabManager.Create<TestEntity>()
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
		                                           RANDOM.Next(-128, 128))));
			}

			window.GetMice()[0].MouseMove += (_, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			var mouse = Window.GetMice()[0];
			_freeCamera.Update(Camera, ref mouse, delta);
			
			this.UpdateKeyBindings(_keyBindings);
		}
	}
}