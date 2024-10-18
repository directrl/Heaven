using Coelum.Graphics;
using Coelum.Graphics.Camera;
using Coelum.Graphics.Object;
using Coelum.Graphics.Scene;
using Coelum.Graphics.Texture;
using Coelum.Input;
using Coelum.UI;
using Coelum.World;
using Coelum.World.Components;
using Coelum.World.Entity;
using Coelum.World.Queries;
using ImGuiNET;
using Playground.Entity;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class WorldTest : Scene3D {

		private static readonly Random RANDOM = new();

		private World _world;
		
		private KeyBindings _keyBindings;
		private FreeCamera _freeCamera;

		private DebugUI? _debugOverlay;

		private KeyBinding _spawnKeybind;
		private KeyBinding _interactKeybind;

		private int _entitiesPerSpawn = 10;
		
		public WorldTest() : base("world-test") {
			_world = new();
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);

			_spawnKeybind = _keyBindings.Register(new("spawn", Key.T));
			_interactKeybind = _keyBindings.Register(new("interact", Key.Y));
			
			ShaderOverlays.AddRange(Texture2D.OVERLAYS);
			ShaderOverlays.AddRange(TextureArray.OVERLAYS);
			ShaderOverlays.AddRange(InstancedObject<Object3D>.OVERLAYS);
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			Camera = new PerspectiveCamera(window) {
				FOV = 60
			};

			if(_debugOverlay == null) {
				_debugOverlay = new(this);
				_debugOverlay.AdditionalInfo += (delta, args) => {
					if(ImGui.Begin("World information")) {
						ImGui.Text($"Entity count: {_world.Entities.Count}");
						ImGui.SliderInt("Entities per single spawn", ref _entitiesPerSpawn, 1, 100);
						ImGui.End();
					}
				};
			}
			
			window.GetMice()[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};

		#region World
			_world = new();
		#endregion
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			
			var mouse = Window.Input.Mice[0];
			_freeCamera.Update(Camera, ref mouse, delta);

			if(_spawnKeybind.Pressed) {
				for(int i = 0; i < _entitiesPerSpawn; i++) {
					var entity = new TestEntity() {
						Position = new(RANDOM.Next(-64, 64), RANDOM.Next(-64, 64), RANDOM.Next(-64, 64)),
					};
					entity.Model.Material.Color = new(
						RANDOM.NextSingle(),
						RANDOM.NextSingle(),
						RANDOM.NextSingle(),
						Math.Clamp(RANDOM.Next(-64, 64), 0.5f, 1.0f)
					);
					
					entity.Spawn(_world);
				}
			}

			if(_interactKeybind.Pressed) {
				EntityQueries.QueryByComponentAll<IInteractable>(_world, entity => {
					((IInteractable) entity).Interact();
				});
			}
			
			this.UpdateKeyBindings(_keyBindings);

			ComponentQueries.QueryByComponentAll(_world, (ITickable tickable) => {
				tickable.Update(delta);
			});
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			gl.Disable(EnableCap.CullFace);

			foreach(var entity in _world.Entities.Values) {
				entity.Load(PrimaryShader);
				entity.Render();
			}
		}
	}
}