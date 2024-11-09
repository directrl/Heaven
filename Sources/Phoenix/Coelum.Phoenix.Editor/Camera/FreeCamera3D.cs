using System.Numerics;
using Coelum.Common.Input;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Input;
using Silk.NET.Input;

namespace Coelum.Phoenix.Editor.Camera {
	
	public class FreeCamera3D {

		public static float CameraSensitivity { get; set; } = 0.1f;
		public static float CameraSpeed { get; set; } = 10;

	#region Key bindings
		private KeyBinding _cursorToggle;
		
		private KeyBinding _cameraUp;
		private KeyBinding _cameraDown;
		private KeyBinding _cameraForward;
		private KeyBinding _cameraBackward;
		private KeyBinding _cameraLeft;
		private KeyBinding _cameraRight;
	#endregion
		
		private Vector2 _lastCursorPosition;
		private bool _cursorEnabled = false;

		private PhoenixScene _scene;
		
		public Camera3D Camera { get; set; }

		public FreeCamera3D(Camera3D camera, PhoenixScene scene, KeyBindings keyBindings) {
			Tests.Assert(scene.Window != null);
			
			Camera = camera;
			_scene = scene;

			_cursorToggle = keyBindings.Register(new("fc_cursor_toggle", Key.R));
			
			_cameraUp = keyBindings.Register(new("fc_camera_up", Key.Space));
			_cameraDown = keyBindings.Register(new("fc_camera_down", Key.ShiftLeft));
			_cameraForward = keyBindings.Register(new("fc_camera_forward", Key.W));
			_cameraBackward = keyBindings.Register(new("fc_camera_backward", Key.S));
			_cameraLeft = keyBindings.Register(new("fc_camera_left", Key.A));
			_cameraRight = keyBindings.Register(new("fc_camera_right", Key.D));

			scene.Window.GetMice()[0].MouseMove += (_, pos) => {
				OnMouseMove(pos);
			};

			scene.Update += OnUpdate;
		}
		
		private void OnUpdate(float delta) {
			if(_cursorToggle.Pressed) {
				_cursorEnabled = !_cursorEnabled;
				
				// TODO maybe Window shouldn't be nullable?
				_scene.Window.GetMice()[0].Cursor.CursorMode =
					_cursorEnabled 
						? CursorMode.Normal 
						: CursorMode.Disabled;
			}

			if(_cursorEnabled) return;

			if(_cameraUp.Down) {
				// TODO make it possible to only query by child class e.g. GetComponent<Transform3D>
				Camera.GetComponent<Transform, Transform3D>().Position += new Vector3(0, CameraSpeed * delta, 0);
			}
			
			if(_cameraDown.Down) {
				Camera.GetComponent<Transform, Transform3D>().Position -= new Vector3(0, CameraSpeed * delta, 0);
			}
			
			if(_cameraForward.Down) {
				Camera.MoveForward(CameraSpeed * delta);
			}
			
			if(_cameraBackward.Down) {
				Camera.MoveBackward(CameraSpeed * delta);
			}
			
			if(_cameraLeft.Down) {
				Camera.MoveLeft(CameraSpeed * delta);
			}
			
			if(_cameraRight.Down) {
				Camera.MoveRight(CameraSpeed * delta);
			}
			
			Console.WriteLine(Camera.GetComponent<Transform, Transform3D>().Position);
			Console.WriteLine(Camera.GetComponent<Transform, Transform3D>().Rotation);
		}
		
		private void OnMouseMove(Vector2 mousePosition) {
			if(!_cursorEnabled) return;
			
			if(_lastCursorPosition == default) {
				_lastCursorPosition = mousePosition;
			} else {
				float deltaX = (mousePosition.X - _lastCursorPosition.X) * CameraSensitivity;
				float deltaY = (mousePosition.Y - _lastCursorPosition.Y) * CameraSensitivity;
				_lastCursorPosition = mousePosition;

				Camera.Yaw += deltaX;
				Camera.Pitch -= deltaY;

				Camera.Yaw = Camera.Yaw % 360.0f;
				Camera.Pitch = Math.Clamp(Camera.Pitch, -89.9f, 89.9f);
			}
		}
	}
}