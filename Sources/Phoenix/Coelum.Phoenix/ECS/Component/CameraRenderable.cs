using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	internal class CameraRenderable : Renderable {
		
		public Node? Owner { get; set; }
		public CameraBase Camera { get; }
		
		public CameraRenderable(CameraBase camera) {
			Camera = camera;
		}

		public void Render(ShaderProgram shader) {
			Camera.RecalculateViewMatrix();

			shader.SetUniform("projection", Camera.ProjectionMatrix);
			shader.SetUniform("view", Camera.ViewMatrix);

			if(Camera is Camera3D) {
				shader.SetUniform("camera_pos",
				                  Camera.GetComponent<Transform, Transform3D>().GlobalPosition);
			} else {
				shader.SetUniform("camera_pos",
				                  new Vector3(Camera.GetComponent<Transform, Transform2D>().GlobalPosition, 0));
			}
		}
	}
}