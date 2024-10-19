using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Coelum.Core;
using Coelum.Graphics;
using Coelum.Graphics.Camera;
using Coelum.Graphics.Object;
using Coelum.Graphics.Scene;
using Coelum.Graphics.Texture;
using Coelum.Input;
using Coelum.LanguageExtensions;
using Coelum.UI;
using Coelum.World;
using Coelum.World.Components;
using Coelum.World.Entity;
using Coelum.World.Object;
using Coelum.World.Queries;
using ImGuiNET;
using Playground.Entity;
using Serilog;
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

		private float _worldGenHeightMultiplier = 20;
		private int _worldGenSize = 64;
		private int _worldGenSizeChunks = 1;
		private bool _worldGenAlignToGrid = false;
		private float _worldGenFrequency = 0.008f;
		
		public WorldTest() : base("world-test") {
			_world = new();
			_keyBindings = new(Id);
			_freeCamera = new(_keyBindings);
			
			this.SetupKeyBindings(_keyBindings);

			_spawnKeybind = _keyBindings.Register(new("spawn", Key.T));
			_interactKeybind = _keyBindings.Register(new("interact", Key.Y));
			
			//ShaderOverlays.AddRange(Texture2D.OVERLAYS); TODO fix material colors not applying
			ShaderOverlays.AddRange(TextureArray.OVERLAYS);
			ShaderOverlays.AddRange(InstancedObject<Object3D>.OVERLAYS);
		}

		private void RebuildWorld() {
			_world.Chunks.Clear();
			_world = new();
			
			var noise = new FastNoiseLite(RANDOM.Next());
			noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
			noise.SetFrequency(_worldGenFrequency);

			int left = _worldGenSize * _worldGenSize;
			
			int chunksTotal = _worldGenSizeChunks * _worldGenSizeChunks;
			int chunkI = 1;
			
			Playground.AppLogger.Information("BEGIN WORLD REBUILD");
			var swTotal = Stopwatch.StartNew();
			
			for(int chunkX = 0; chunkX < _worldGenSizeChunks; chunkX++) {
				for(int chunkZ = 0; chunkZ < _worldGenSizeChunks; chunkZ++) {
					Playground.AppLogger.Information($"Constructing chunk {chunkI}/{chunksTotal}");
					var sw = Stopwatch.StartNew();
					
					var chunk = _world.CreateChunk(new(chunkX, 0, chunkZ));
					
					//for(int y = 0; y < 16; y++)
					for(int x = 0; x < _worldGenSize; x++) {
						if(x >= Chunk.SIZE_X) break;
						
						for(int z = 0; z < _worldGenSize; z++) {
							if(z >= Chunk.SIZE_Z) break;
							
							var pos = new WorldCoord(chunk);
							pos.WorldPosition.X += x;
							pos.WorldPosition.Y = 0;
							pos.WorldPosition.Z += z;
							
							float y = noise.GetNoise(pos.X, pos.Z);
							y = (y + 1) / 2;
							
							Vector4 color;
							
							if(y < 0.2) color = Color.FromArgb(50, 130, 220).ToVector4();
							else if(y > 0.2 && y < 0.6) color = Color.FromArgb(40, 90, 30).ToVector4();
							else if(y > 0.6 && y < 0.9) color = Color.FromArgb(90, 90, 90).ToVector4();
							else color = Color.FromArgb(240, 240, 240).ToVector4();
							
							y *= _worldGenHeightMultiplier;
							pos.Y = (int) y;
							
							var obj = new VoxelObject(
								_world,
								chunk,
								pos
							)/* {
								Position = new(pos.X, y, pos.Z),
							}*/;
							obj.Model.Material.Color = color;

							if(!_worldGenAlignToGrid) {
								obj.Position = new(pos.X, y, pos.Z);
							}
							
							_world.PlaceObject(obj);
						}
					}

					sw.Stop();
					Playground.AppLogger.Information($"Constructing took {sw.ElapsedMilliseconds}ms");
					
					Playground.AppLogger.Information($"Building chunk {chunkI}/{chunksTotal}");
					sw.Restart();
					chunk.Build();
					sw.Stop();
					Playground.AppLogger.Information($"Building took {sw.ElapsedMilliseconds}ms");

					chunkI++;
				}
			}
			
			swTotal.Stop();
			Playground.AppLogger.Information($"FINISHED IN {swTotal.ElapsedMilliseconds}ms");
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			Camera = new PerspectiveCamera(window) {
				FOV = 60,
				ZFar = 10000
			};

			if(_debugOverlay == null) {
				_debugOverlay = new(this);
				_debugOverlay.AdditionalInfo += (delta, args) => {
					if(ImGui.Begin("World information", ImGuiWindowFlags.AlwaysAutoResize)) {
						ImGui.Text($"Camera position: {Camera?.Position.ToString("n2") ?? "Unknown"}");
						ImGui.SliderFloat("Camera speed", ref FreeCamera.CAMERA_SPEED, 10, 100);
						ImGui.Separator();
						
						ImGui.Text($"Chunk count: {_world.Chunks.Count}");

						int objectCount = 0;
						foreach(var chunk in _world.Chunks.Values) {
							objectCount += chunk.NodeCount;
						}
						
						ImGui.Text($"Total world objects: {objectCount}");
						ImGui.Separator();
						ImGui.Text($"Entity count: {_world.Entities.Count}");
						ImGui.SliderInt("Entities per single spawn", ref _entitiesPerSpawn, 1, 100);
						ImGui.End();
					}
					
					if(ImGui.Begin("World generation", ImGuiWindowFlags.AlwaysAutoResize)) {
						ImGui.SliderFloat("Height multiplier", ref _worldGenHeightMultiplier, 1, 256);
						ImGui.SliderInt("Generation size (square per chunk)", ref _worldGenSize, 3, 256);
						ImGui.SliderInt("Generation size (chunks)", ref _worldGenSizeChunks, 1, 16);
						ImGui.SliderFloat("Noise frequency", ref _worldGenFrequency, 0.001f, 0.1f);
						ImGui.Checkbox("Align to grid", ref _worldGenAlignToGrid);

						if(ImGui.Button("Generate")) {
							RebuildWorld();
						}
						
						ImGui.End();
					}
				};
			}
			
			window.GetMice()[0].MouseMove += (mouse, pos) => {
				_freeCamera.CameraMove(Camera, pos);
			};
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
					
					_world.Spawn(entity);
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
			//gl.Disable(EnableCap.CullFace);

			foreach(var chunk in _world.Chunks.Values) {
				chunk.Load(PrimaryShader);
				chunk.Render();
			}
			
			foreach(var entity in _world.Entities.Values) {
				entity.Load(PrimaryShader);
				entity.Render();
			}
		}
	}
}