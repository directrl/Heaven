using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.Camera {
	
	public abstract class CameraBase : Node {
		
		public float Width { get; internal set; }
		public float Height { get; internal set; }
		
		public Matrix4x4 ProjectionMatrix { get; protected set; }
		public Matrix4x4 ViewMatrix { get; protected set; }
		
		public Matrix4x4 InverseProjectionMatrix { get; protected set; }
		public Matrix4x4 InverseViewMatrix { get; protected set; }

		protected CameraBase() { }
		
		internal abstract void RecalculateProjectionMatrix();
		internal abstract void RecalculateViewMatrix();
		
		public void Load(ShaderProgram shader) {
			var ubo = shader.CreateBufferBinding<CameraMatrices>();
			
			RecalculateViewMatrix();
			
			ubo.Projection = ProjectionMatrix;
			ubo.View = ViewMatrix;

			ubo.CameraPos = this is Camera3D
				? GetComponent<Transform, Transform3D>().GlobalPosition
				: new(GetComponent<Transform, Transform2D>().GlobalPosition, 0);
			
			ubo.Upload();
		}
	}
}