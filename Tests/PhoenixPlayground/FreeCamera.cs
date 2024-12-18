using System.Numerics;
using Coelum.Phoenix.Camera;
using Coelum.Common.Input;
using Coelum.Phoenix.ECS.Components;
using Silk.NET.Input;

namespace PhoenixPlayground {
	
	public class FreeCamera {
		
		private const float CAMERA_SENSITIVITY = 0.1f;
		private const float CAMERA_SPEED = 10f;
		
		private Vector2 _lastMousePosition;
		private bool _cursorToggle = false;
		
		private KeyBindings _keyBindings;
		
		private KeyBinding _cursorToggleKeyBinding;
		
		private KeyBinding _cameraUp;
		private KeyBinding _cameraDown;
		private KeyBinding _cameraForward;
		private KeyBinding _cameraBackward;
		private KeyBinding _cameraLeft;
		private KeyBinding _cameraRight;

		public FreeCamera(KeyBindings keyBindings) {
			_keyBindings = keyBindings;
			
			_cursorToggleKeyBinding = keyBindings.Register(new("cursor_toggle", Key.G));
			
			_cameraUp = keyBindings.Register(new("camera_up", Key.Space));
			_cameraDown = keyBindings.Register(new("camera_down", Key.ShiftLeft));
			_cameraForward = keyBindings.Register(new("camera_forward", Key.W));
			_cameraBackward = keyBindings.Register(new("camera_backward", Key.S));
			_cameraLeft = keyBindings.Register(new("camera_left", Key.A));
			_cameraRight = keyBindings.Register(new("camera_right", Key.D));
		}

		public void Update(Camera3D camera, ref IMouse mouse, float delta, bool noMove = false) {
			if(!_keyBindings.Enabled) return;
			
			if(_cursorToggleKeyBinding.Pressed) {
				_cursorToggle = !_cursorToggle;
				if(!_cursorToggle) _lastMousePosition = default;
			}

			mouse.Cursor.CursorMode = _cursorToggle
				? CursorMode.Disabled
				: CursorMode.Normal;

			if(noMove) return;

			if(_cameraUp.Down) {
				camera.GetComponent<Transform, Transform3D>().Position += new Vector3(0, CAMERA_SPEED * delta, 0);
				//camera.MoveUp(CAMERA_SPEED * delta);
			}
			
			if(_cameraDown.Down) {
				camera.GetComponent<Transform, Transform3D>().Position -= new Vector3(0, CAMERA_SPEED * delta, 0);
				//camera.MoveDown(CAMERA_SPEED * delta);
			}
			
			if(_cameraForward.Down) {
				//camera.Position.Z += CAMERA_SPEED * delta;
				camera.MoveForward(CAMERA_SPEED * delta);
			}
			
			if(_cameraBackward.Down) {
				//camera.Position.Z -= CAMERA_SPEED * delta;
				camera.MoveBackward(CAMERA_SPEED * delta);
			}
			
			if(_cameraLeft.Down) {
				//camera.Position.X -= CAMERA_SPEED * delta;
				camera.MoveLeft(CAMERA_SPEED * delta);
			}
			
			if(_cameraRight.Down) {
				//camera.Position.X += CAMERA_SPEED * delta;
				camera.MoveRight(CAMERA_SPEED * delta);
			}
		}
		
		public void CameraMove(Camera3D camera, Vector2 mousePosition) {
			if(!_cursorToggle) return;
			
			if(_lastMousePosition == default) {
				_lastMousePosition = mousePosition;
			} else {
				var deltaX = (mousePosition.X - _lastMousePosition.X) * CAMERA_SENSITIVITY;
				var deltaY = (mousePosition.Y - _lastMousePosition.Y) * CAMERA_SENSITIVITY;
				_lastMousePosition = mousePosition;

				camera.Yaw += deltaX;
				camera.Pitch -= deltaY;

				camera.Pitch = Math.Clamp(camera.Pitch, -89.9f, 89.9f);
			}
		}
	}
}