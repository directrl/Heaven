using System.Numerics;
using Coeli.Graphics.OpenGL;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Camera {
	
	public class Camera2D {

		private Vector3 _direction = new();
		private Vector3 _front = new(0.0f, 0.0f, 1.0f);
		private Vector3 _up = Vector3.UnitY;
		
		protected float Width { get; private set; }
		protected float Height { get; private set; }

		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }

		public Vector2 Position = new();

		public Camera2D(Window window) {
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

		public void Load(ShaderProgram shader) {
			RecalculateViewMatrix();
			
			shader.SetUniform("projection", ProjectionMatrix);
			shader.SetUniform("view", ViewMatrix);
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