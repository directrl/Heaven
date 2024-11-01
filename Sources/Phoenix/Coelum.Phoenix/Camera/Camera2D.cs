using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Camera {
	
	public class Camera2D : Node, Renderable {

		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;
		
		protected float Width { get; private set; }
		protected float Height { get; private set; }

		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }

		public Camera2D(SilkWindow window) { // TODO
			Components = new() {
				{ typeof(Renderable), this },
				{ typeof(Transform2D), new Transform2D() }
			};
			
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

		public void Render(ShaderProgram shader) {
			RecalculateViewMatrix();
			
			shader.SetUniform("projection", ProjectionMatrix);
			shader.SetUniform("view", ViewMatrix);
		}
		
		public void RecalculateProjectionMatrix() {
			var t2d = GetComponent<Transform2D>();
			
			ViewMatrix = Matrix4x4.CreateLookAt(
				new(t2d.Position.X, t2d.Position.Y, 0),
				new Vector3(t2d.Position.X, t2d.Position.Y, 0) + _front,
				_up);
		}

		public void RecalculateViewMatrix() {
			ProjectionMatrix = Matrix4x4.CreateOrthographic(
				-Width,
				-Height,
				0.0f,
				1.0f
			);
		}
	}
}