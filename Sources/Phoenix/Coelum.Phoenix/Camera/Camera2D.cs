using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Camera {
	
	public class Camera2D : CameraBase {

		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;
		
		public Camera2D(SilkWindow window)
			: this(window.SilkImpl.FramebufferSize.X, window.SilkImpl.FramebufferSize.Y) {
			
			window.SilkImpl.FramebufferResize += size => {
				Width = size.X;
				Height = size.Y;
				
				RecalculateProjectionMatrix();
			};
		}

		public Camera2D(float width, float height) {
			AddComponent<Transform>(new Transform2D());
			
			Width = width;
			Height = height;
			
			RecalculateProjectionMatrix();
			RecalculateViewMatrix();
		}

		public void Render(ShaderProgram shader) {
			RecalculateViewMatrix();
			
			shader.SetUniform("projection", ProjectionMatrix);
			shader.SetUniform("view", ViewMatrix);
		}
		
		internal override void RecalculateProjectionMatrix() {
			var t2d = GetComponent<Transform, Transform2D>();
			
			ViewMatrix = Matrix4x4.CreateLookAt(
				new(t2d.Position.X, t2d.Position.Y, 0),
				new Vector3(t2d.Position.X, t2d.Position.Y, 0) + _front,
				_up);
			
			Matrix4x4.Invert(ViewMatrix, out var ivm);
			InverseViewMatrix = ivm;
		}

		internal override void RecalculateViewMatrix() {
			ProjectionMatrix = Matrix4x4.CreateOrthographic(
				-Width,
				-Height,
				0.0f,
				1.0f
			);
			
			Matrix4x4.Invert(ProjectionMatrix, out var ipm);
			InverseProjectionMatrix = ipm;
		}
	}
}