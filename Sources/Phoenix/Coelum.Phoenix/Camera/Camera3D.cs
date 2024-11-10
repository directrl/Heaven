using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;

namespace Coelum.Phoenix.Camera {
	
	public abstract class Camera3D : CameraBase {

		protected static float Z_NEAR = 0.01f;
		protected static float Z_FAR = 1000f;
		
		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;

		public float Yaw {
			get => GetComponent<Transform, Transform3D>().Rotation.Y;
			set {
				var t3d = GetComponent<Transform, Transform3D>();

				t3d.Rotation = t3d.Rotation with {
					Y = value
				};
				
				_direction.X = MathF.Cos(t3d.Rotation.Y.ToRadians()) * MathF.Cos(t3d.Rotation.X.ToRadians());
				_direction.Z = MathF.Sin(t3d.Rotation.Y.ToRadians()) * MathF.Cos(t3d.Rotation.X.ToRadians());
				_front = Vector3.Normalize(_direction);
			}
		}
		
		public float Pitch {
			get => GetComponent<Transform, Transform3D>().Rotation.X;
			set {
				var t3d = GetComponent<Transform, Transform3D>();
				
				t3d.Rotation = t3d.Rotation with {
					X = value
				};
				
				_direction.X = MathF.Cos(t3d.Rotation.Y.ToRadians()) * MathF.Cos(t3d.Rotation.X.ToRadians());
				_direction.Y = MathF.Sin(t3d.Rotation.X.ToRadians());
				_direction.Z = MathF.Sin(t3d.Rotation.Y.ToRadians()) * MathF.Cos(t3d.Rotation.X.ToRadians());
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

		protected Camera3D() {
			AddComponent<Transform>(new Transform3D());
			// TODO replace with billboard image
			// TODO do not render for current camera
			AddComponent<Renderable>(new ModelRenderable(ModelLoader.Load(Module.RESOURCES[ResourceType.MODEL, "camera.glb"])));
			
			// Width = window.SilkImpl.FramebufferSize.X;
			// Height = window.SilkImpl.FramebufferSize.Y;
			
			RecalculateProjectionMatrix();
			RecalculateViewMatrix();
		}

		public void MoveUp(float amount) => GetComponent<Transform, Transform3D>().Position += _up = Vector3.Multiply(ViewMatrix.PositiveY(), amount);
		public void MoveDown(float amount) => GetComponent<Transform, Transform3D>().Position -= _up = Vector3.Multiply(ViewMatrix.PositiveY(), amount);
		public void MoveLeft(float amount) => GetComponent<Transform, Transform3D>().Position -= Vector3.Multiply(ViewMatrix.PositiveX(), amount);
		public void MoveRight(float amount) => GetComponent<Transform, Transform3D>().Position += Vector3.Multiply(ViewMatrix.PositiveX(), amount);
		public void MoveForward(float amount) => GetComponent<Transform, Transform3D>().Position -= _direction = Vector3.Multiply(ViewMatrix.PositiveZ(), amount);
		public void MoveBackward(float amount) => GetComponent<Transform, Transform3D>().Position += _direction = Vector3.Multiply(ViewMatrix.PositiveZ(), amount);

		internal override void RecalculateViewMatrix() {
			var t3d = GetComponent<Transform, Transform3D>();
			
			ViewMatrix = Matrix4x4.CreateLookAt(
				t3d.GlobalPosition,
				t3d.GlobalPosition + _front,
				_up
			);
			
			Matrix4x4.Invert(ViewMatrix, out var ivm);
			InverseViewMatrix = ivm;
		}
	}
}