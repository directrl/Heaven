using System.Numerics;
using Coelum.Graphics.OpenGL;
using Coelum.LanguageExtensions;

namespace Coelum.Graphics.Camera {
	
	public abstract class Camera3D {
		
		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;
		
		protected float Width { get; private set; }
		protected float Height { get; private set; }

		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 InverseProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }
		public Matrix4x4 InverseViewMatrix { get; protected set; }

		public Vector3 Position = new();

		public float ZNear = 0.01f;
		public float ZFar = 1000f;

		private float _yaw;
		public float Yaw {
			get => _yaw;
			set {
				_yaw = value;
				_direction.X = MathF.Cos(_yaw.ToRadians()) * MathF.Cos(_pitch.ToRadians());
				_direction.Z = MathF.Sin(_yaw.ToRadians()) * MathF.Cos(_pitch.ToRadians());
				_front = Vector3.Normalize(_direction);
			}
		}
		
		private float _pitch;
		public float Pitch {
			get => _pitch;
			set {
				_pitch = value;
				_direction.X = MathF.Cos(_yaw.ToRadians()) * MathF.Cos(_pitch.ToRadians());
				_direction.Y = MathF.Sin(_pitch.ToRadians());
				_direction.Z = MathF.Sin(_yaw.ToRadians()) * MathF.Cos(_pitch.ToRadians());
				_front = Vector3.Normalize(_direction);
			}
		}

		private float _fov = 1.0f;
		public float FOV {
			get => _fov;
			set {
				_fov = value;
				RecalculateProjectionMatrix();
			}
		}

		protected Camera3D(Window window) {
			Width = window.SilkImpl.FramebufferSize.X;
			Height = window.SilkImpl.FramebufferSize.Y;
			
			RecalculateProjectionMatrix();
			RecalculateViewMatrix();

			window.SilkImpl.FramebufferResize += size => {
				Width = size.X;
				Height = size.Y;
				
				RecalculateProjectionMatrix();
			};
		}

		public void MoveUp(float amount) => Position += _up = Vector3.Multiply(ViewMatrix.PositiveY(), amount);
		public void MoveDown(float amount) => Position -= _up = Vector3.Multiply(ViewMatrix.PositiveY(), amount);
		public void MoveLeft(float amount) => Position -= Vector3.Multiply(ViewMatrix.PositiveX(), amount);
		public void MoveRight(float amount) => Position += Vector3.Multiply(ViewMatrix.PositiveX(), amount);
		public void MoveForward(float amount) => Position -= _direction = Vector3.Multiply(ViewMatrix.PositiveZ(), amount);
		public void MoveBackward(float amount) => Position += _direction = Vector3.Multiply(ViewMatrix.PositiveZ(), amount);

		public void Load(ShaderProgram shader) {
			RecalculateViewMatrix();
			
			shader.SetUniform("projection", ProjectionMatrix);
			shader.SetUniform("view", ViewMatrix);
		}

		protected abstract void RecalculateProjectionMatrix();

		protected void RecalculateViewMatrix() {
			ViewMatrix = Matrix4x4.CreateLookAt(
				Position,
				Position + _front,
				_up
			);
			
			Matrix4x4.Invert(ViewMatrix, out var ivm);
			InverseViewMatrix = ivm;
		}
	}
}