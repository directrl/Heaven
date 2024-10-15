using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Coeli.Core;
using Coeli.Graphics;
using Coeli.Graphics.Object;
using Coeli.Graphics.Scene;
using Coeli.Input;
using Coeli.Resources;
using Coeli.UI;
using ImGuiNET;
using Playground.Scenes.VoxelSpace;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Playground.Scenes {
	
	public class VoxelSpaceScene : Scene2D {

		private readonly KeyBindings _keyBindings;

	#region KeyBindings
		private readonly KeyBinding _up;
		private readonly KeyBinding _down;
		private readonly KeyBinding _left;
		private readonly KeyBinding _right;

		private readonly KeyBinding _forward;
		private readonly KeyBinding _backward;
		private readonly KeyBinding _moveLeft;
		private readonly KeyBinding _moveRight;
		private readonly KeyBinding _yawLeft;
		private readonly KeyBinding _yawRight;
		private readonly KeyBinding _moveUp;
		private readonly KeyBinding _moveDown;
		private readonly KeyBinding _pitchUp;
		private readonly KeyBinding _pitchDown;
	#endregion

		private Mesh? _pixel;
		private readonly List<Object2D> _objects = new();

		private DebugUI _debugOverlay;
		private ImGuiOverlay _overlay;

	#region Voxel Space
		private Vector3[,] _colorMap;
		private int _cMw, _cMh;
		
		private Vector3[,] _heightMap;
		private int _hMw, _hMh;

	#region Settings
		private const int VS_DISTANCE = 128;
		private const int VS_SCALE_HEIGHT = 100;

		private Vector2 _pos = new(400, 450);
		private float _phi = 0;
		private float _height = 150;
		private float _horizon = 300;
	#endregion
	#endregion
		
		public VoxelSpaceScene() : base("voxel-space") {
			_keyBindings = new(Id);
			_up = _keyBindings.Register(new("up", Key.Up));
			_down = _keyBindings.Register(new("down", Key.Down));
			_left = _keyBindings.Register(new("left", Key.Left));
			_right = _keyBindings.Register(new("right", Key.Right));
			_up = _keyBindings.Register(new("up", Key.Up));
			
			_forward = _keyBindings.Register(new("f", Key.W));
			_backward = _keyBindings.Register(new("b", Key.S));
			_moveLeft = _keyBindings.Register(new("ml", Key.A));
			_moveRight = _keyBindings.Register(new("mr", Key.D));
			_yawLeft = _keyBindings.Register(new("yl", Key.Q));
			_yawRight = _keyBindings.Register(new("yr", Key.E));
			_moveUp = _keyBindings.Register(new("mu", Key.R));
			_moveDown = _keyBindings.Register(new("md", Key.F));
			_pitchUp = _keyBindings.Register(new("pu", Key.T));
			_pitchDown = _keyBindings.Register(new("pd", Key.G));
			
			Heaven.AppLogger?.Information("Loading color map");
			MapLoader.DoParse(Heaven.AppResources[Resource.Type.TEXTURE, "C10W"].GetStream(), out _colorMap, ref _cMw, ref _cMh);
			Heaven.AppLogger?.Information("Loading height map");
			MapLoader.DoParse(Heaven.AppResources[Resource.Type.TEXTURE, "D10"].GetStream(), out _heightMap, ref _hMw, ref _hMh);
			
			Heaven.AppLogger?.Information($"Color map: ({_cMw}, {_cMh}), Height map: ({_hMw}, {_hMh})");

			ClearColor = Color.FromArgb(0, 0, 0, 0);
		}

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			Camera = new(window);
			_debugOverlay = new(this);
			//_overlay = new(this);

			if(_pixel == null) {
				_pixel = new(PrimitiveType.TriangleStrip,
					new float[] {
						0, 0, 0,
						0, 1, 0,
						1, 0, 0,
						1, 1, 0
					},
					new uint[] {
						0, 2, 1, 3
					},
					null, null);
			}
			
			_objects.Add(new Object2D {
				Meshes = new[] { _pixel },
				Scale = new(100, 100)
			});

			/*_overlay.Render += (delta, args) => {
				if(ImGui.Begin("Information", ImGuiWindowFlags.AlwaysAutoResize)) {
					ImGui.Text($"First object position: {_objects[0].Position}");
					ImGui.Text($"Last object position: {_objects.Last().Position}");
					
					ImGui.End();
				}
			};*/

			_debugOverlay.AdditionalInfo += (delta, args) => {
				ImGui.Text($"Object count: {Renderer.ObjectCount}");
			};
			
			window.Input.Keyboards[0].KeyUp += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Release, k);
			};

			window.Input.Keyboards[0].KeyDown += (kb, k, sc) => {
				_keyBindings.Input(KeyAction.Press, k);
			};
		}

		public override void OnFixedUpdate(float delta) {
			base.OnFixedUpdate(delta);

			float move = 100 * delta;

			if(_up.Down) _objects[0].Position.Y -= move;
			if(_down.Down) _objects[0].Position.Y += move;
			if(_left.Down) _objects[0].Position.X -= move;
			if(_right.Down) _objects[0].Position.X += move;

			float cM = 15 * delta;

			if(_forward.Down) _pos.Y -= cM;
			if(_backward.Down) _pos.Y += cM;
			if(_moveLeft.Down) _pos.X -= cM;
			if(_moveRight.Down) _pos.X += cM;
			if(_yawLeft.Down) _phi += cM / MathF.PI / 10;
			if(_yawRight.Down) _phi -= cM / MathF.PI / 10;
			if(_moveUp.Down) _height += 5;
			if(_moveDown.Down) _height -= 5;
			if(_pitchUp.Down) _horizon += 20;
			if(_pitchDown.Down) _horizon -= 20;
			
			_keyBindings.Update(Window.Input.Keyboards[0]);
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
		}

		public override void OnUnload() {
			base.OnUnload();
			_keyBindings.Dispose();
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			
			Renderer.Render(Window,
				_pos, _phi, _height, _horizon, VS_SCALE_HEIGHT, VS_DISTANCE,
				ref _colorMap, ref _heightMap,
				ref _cMw, ref _cMh, ref _hMw, ref _hMh,
				MainShader, ref _pixel);
			
			// MainShader.SetUniform("color", new Vector4(1.0f, 1.0f, 0.0f, 1.0f));
			//
			// foreach(var o in _objects) {
			// 	o.Render(MainShader);
			// }

			foreach(var kv in Renderer.Objects) {
				MainShader.SetUniform("color", kv.Value);
				kv.Key.Render(MainShader);
			}
		}
	}
}