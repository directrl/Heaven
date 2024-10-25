using System.Numerics;
using Coelum.Graphics.Phoenix.Node;
using Coelum.Graphics.Phoenix.OpenGL;

namespace Coelum.Graphics.Phoenix.Camera {
	
	public class Camera2D : Node2D {

		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;
		
		protected float Width { get; private set; }
		protected float Height { get; private set; }

		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }

		public Camera2D(SilkWindow window) {
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

		public override void Render(ShaderProgram shader) {
			RecalculateViewMatrix();
			
			shader.SetUniform("projection", ProjectionMatrix);
			shader.SetUniform("view", ViewMatrix);
			
			base.Load(shader);
		}
		
		protected void RecalculateProjectionMatrix() {
			ViewMatrix = Matrix4x4.CreateLookAt(
				new(Position.X, Position.Y, 0),
				new Vector3(Position.X, Position.Y, 0) + _front,
				_up);
		}

		protected void RecalculateViewMatrix() {
			ProjectionMatrix = Matrix4x4.CreateOrthographic(
				-Width,
				-Height,
				0.0f,
				1.0f
			);
		}
	}
}