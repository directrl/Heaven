using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Camera : INodeComponent {
		
		public Node? Owner { get; set; }
		public CameraBase TheCamera { get; }
		
		public Camera(CameraBase camera) {
			TheCamera = camera;
		}

		public void Load(CameraMatrices ubo) {
			TheCamera.RecalculateViewMatrix();
			
			ubo.Projection = TheCamera.ProjectionMatrix;
			ubo.View = TheCamera.ViewMatrix;

			if(TheCamera is Camera3D) {
				ubo.CameraPos = TheCamera.GetComponent<Transform, Transform3D>().GlobalPosition;
			} else {
				ubo.CameraPos = new(TheCamera.GetComponent<Transform, Transform2D>().GlobalPosition, 0);
			}
		}
	}
}