using System.Numerics;

namespace Coelum.Graphics.Camera {
	
	public class OrthographicCamera : Camera3D {
		
		public OrthographicCamera(Window window) : base(window) { }

		protected override void RecalculateProjectionMatrix() {
			ProjectionMatrix = Matrix4x4.CreateOrthographic(
				Width * FOV / 1000,
				Height * FOV / 1000,
				Z_NEAR,
				Z_FAR
			);
			
			Matrix4x4.Invert(ProjectionMatrix, out var ipm);
			InverseProjectionMatrix = ipm;
		}
	}
}