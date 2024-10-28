using System.Diagnostics;
using System.Drawing;
using Coelum.Common.Input;
using Coelum.Raven;
using Coelum.Raven.Node;
using Coelum.Raven.Node.Component;
using Coelum.Raven.Scene;
using Coelum.Raven.Shader.Cell;
using Coelum.Raven.Window;
using RavenPlayground.Node;
using Silk.NET.Input;

namespace RavenPlayground.Scenes {
	
	public class TestBasicScene : RavenSceneBase {

		private Camera _camera;
		
		private CharNode _player;
		private CharNode _object;
		private LabelNode _fps;
		private HouseNode _house;

		private KeyBindings _keyBindings;
		
		private KeyBinding _up;
		private KeyBinding _down;
		private KeyBinding _left;
		private KeyBinding _right;

		public TestBasicScene() : base("test-basic") {
			_keyBindings = new(Id);
			_up = _keyBindings.Register(new("lalala", (Key) ConsoleKey.V));
		}

		public override void OnLoad(RenderWindow window) {
			base.OnLoad(window);

			_object = new('H') {
				BackgroundColor = Color.Blue,
				Position = new(-10, 10)
			};
			AddChild(_object);
			
			_house = new() {
				Position = new(20, 2)
			};
			_object.AddChild(_house);

			_player = new('@') {
				BackgroundColor = Color.FromArgb(0, 0, 0, 0)
			};
			AddChild(_player);
			
			_fps = new("FPS: NaN");
			AddChild(_fps);
			
			Context.Display.HideCursor();
			_camera = new(Context);
			_player.AddChild(_camera);

			Context.CellShaders.Add(new BlendingShader(Context, BlendingShader.BlendingType.Hard));
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);

			_fps.Text = $"FPS: {1000 / delta}";
			
			//if(Console.KeyAvailable) {
				//System.Diagnostics.Debug.WriteLine(_input.ReadByte());
			//}
			
			//Console.Write(_kb.IsKeyPressed((Key) ConsoleKey.B));
			
			if(_up.Pressed) {
				Debug.Write("pressed");
			}

			if(_up.Down) {
				Debug.Write("down");
			}

			if(Console.KeyAvailable) {
				var key = Console.ReadKey(true);

				switch(key.Key) {
				#region Player
					case ConsoleKey.W:
						_player.Position.Y--;
						
						FindChildrenByComponentParallel((ICollidable collidable) => {
							if(collidable.RelativeAABB.Collides(_player.Position)) {
								_player.Position.Y++;
							}
						});
						break;
					case ConsoleKey.S:
						_player.Position.Y++;
						
						FindChildrenByComponentParallel((ICollidable collidable) => {
							if(collidable.RelativeAABB.Collides(_player.Position)) {
								_player.Position.Y--;
							}
						});
						break;
					case ConsoleKey.A:
						_player.Position.X--;
						
						FindChildrenByComponentParallel((ICollidable collidable) => {
							if(collidable.RelativeAABB.Collides(_player.Position)) {
								_player.Position.X++;
							}
						});
						break;
					case ConsoleKey.D:
						_player.Position.X++;
						
						FindChildrenByComponentParallel((ICollidable collidable) => {
							if(collidable.RelativeAABB.Collides(_player.Position)) {
								_player.Position.X--;
							}
						});
						break;
				#endregion

				#region View position
					case ConsoleKey.R:
						_camera.Position.Y--;
						break;
					case ConsoleKey.G:
						_camera.Position.Y++;
						break;
					case ConsoleKey.F:
						_camera.Position.X--;
						break;
					case ConsoleKey.T:
						_camera.Position.X++;
						break;
				#endregion

				#region View size
					case ConsoleKey.Y:
						_camera.ViewMatrix.M22--;
						break;
					case ConsoleKey.J:
						_camera.ViewMatrix.M22++;
						break;
					case ConsoleKey.H:
						_camera.ViewMatrix.M12--;
						break;
					case ConsoleKey.U:
						_camera.ViewMatrix.M12++;
						break;
				#endregion
					
				#region Clip position
					case ConsoleKey.I:
						_camera.ViewMatrix.M23--;
						break;
					case ConsoleKey.L:
						_camera.ViewMatrix.M23++;
						break;
					case ConsoleKey.K:
						_camera.ViewMatrix.M13--;
						break;
					case ConsoleKey.O:
						_camera.ViewMatrix.M13++;
						break;
				#endregion
					
				#region Clip size
					case ConsoleKey.UpArrow:
						_camera.ViewMatrix.M24--;
						break;
					case ConsoleKey.DownArrow:
						_camera.ViewMatrix.M24++;
						break;
					case ConsoleKey.LeftArrow:
						_camera.ViewMatrix.M14--;
						break;
					case ConsoleKey.RightArrow:
						_camera.ViewMatrix.M14++;
						break;
				#endregion
					case ConsoleKey.Spacebar:
						_camera.RecalculateViewMatrix();
						break;
					case ConsoleKey.Backspace:
						Context.Clear(ref Context.BackBuffer);
						Context.Clear(ref Context.FrontBuffer);
						Context.Display.Clear();
						break;
					default:
						break;
				}
			}
			
			_camera.RecalculateViewMatrix();

			if(((ICollidable) _house).RelativeAABB.Collides(_player.Position)) {
				_player.BackgroundColor = Color.ForestGreen;
			} else {
				_player.BackgroundColor = Color.MediumVioletRed;
			}
		}

		public override void OnUnload() {
			base.OnUnload();
			
			_keyBindings.Dispose();
		}

		public override void OnRender(float delta) {
			/*// view
			Context.DrawRect(
				Context.ViewMatrix.M11, Context.ViewMatrix.M21,
				Context.ViewMatrix.M12, Context.ViewMatrix.M22,
				new() {
					Character = '\u2592',
					ForegroundColor = Color.MediumAquamarine
				}
			);
			
			// clip
			Context.DrawRect(
				Context.ViewMatrix.M13, Context.ViewMatrix.M23,
				Context.ViewMatrix.M14, Context.ViewMatrix.M24,
				new() {
					Character = '\u259a',
					ForegroundColor = Color.Goldenrod
				}
			);*/
			
			base.OnRender(delta);
			
			//Context.DrawBoundingBox(_house, ((ICollidable) _house).AABB);
		}
	}
}