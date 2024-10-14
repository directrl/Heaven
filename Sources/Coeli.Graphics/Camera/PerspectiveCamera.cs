using System.Numerics;
using Coeli.LanguageExtensions;

namespace Coeli.Graphics.Camera {
	
	public class PerspectiveCamera : Camera3D {
		
		public PerspectiveCamera(Window window) : base(window) { }

		protected override void RecalculateProjectionMatrix() {
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