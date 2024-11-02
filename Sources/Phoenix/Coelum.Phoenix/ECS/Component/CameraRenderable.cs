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
		}
	}
}