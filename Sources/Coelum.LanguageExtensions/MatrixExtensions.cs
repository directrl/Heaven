using System.Diagnostics;
using System.Numerics;

namespace Coelum.LanguageExtensions {
	
	public static class MatrixExtensions {
		
		public static Vector3 PositiveX(this Matrix4x4 matrix, ref Vector3 dir) {
			dir.X = matrix.M11;
			dir.Y = matrix.M21;
			dir.Z = matrix.M31;

			Vector3.Normalize(dir);
			return dir;
		}

		public static Vector3 PositiveY(this Matrix4x4 matrix, ref Vector3 dir) {
			dir.X = matrix.M12;
			dir.Y = matrix.M22;
			dir.Z = matrix.M32;

			Vector3.Normalize(dir);
			return dir;
		}
		
		public static Vector3 PositiveZ(this Matrix4x4 matrix, ref Vector3 dir) {
			dir.X = matrix.M13;
			dir.Y = matrix.M23;
			dir.Z = matrix.M33;

			Vector3.Normalize(dir);
			return dir;
		}
		
		public static Vector3 PositiveX(this Matrix4x4 matrix) {
			return Vector3.Normalize(new(matrix.M11, matrix.M21, matrix.M31));
		}

		public static Vector3 PositiveY(this Matrix4x4 matrix) {
			return Vector3.Normalize(new(matrix.M12, matrix.M22, matrix.M32));
		}
		
		public static Vector3 PositiveZ(this Matrix4x4 matrix) {
			return Vector3.Normalize(new(matrix.M13, matrix.M23, matrix.M33));
		}
		
		// adapted from
		// https://github.com/CedricGuillemet/ImGuizmo/blob/e552f632bbb17a0ebf5a91a22900f6f68bac6545/ImGuizmo.cpp#L2520
		public static void ComposeFromComponents(ref this Matrix4x4 matrix,
		                                         Vector3 translation,
		                                         Vector3 rotation,
												 Vector3 scale) {

			// rotation
			var rotationX = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, rotation.X);
			var rotationY = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, rotation.Y);
			var rotationZ = Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, rotation.Z);

			//matrix = rotationX * rotationY * rotationZ;
			matrix = Matrix4x4.CreateScale(scale)
				* (rotationX * rotationY * rotationZ)
				* Matrix4x4.CreateTranslation(translation);
		}
	}
}