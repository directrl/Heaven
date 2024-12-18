using System.Numerics;
using Coelum.LanguageExtensions;

namespace Coelum.Phoenix.Camera {
	
	public class PerspectiveCamera : Camera3D {

		internal override void RecalculateProjectionMatrix() {
			ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(
				FOV.ToRadians(),
				Width / Height,
				Z_NEAR,
				Z_FAR
			);

			Matrix4x4.Invert(ProjectionMatrix, out var ipm);
			InverseProjectionMatrix = ipm;
		}
	}
}