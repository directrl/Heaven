using System.Numerics;

namespace Coelum.LanguageExtensions {
	
	public static class QuaternionExtensions {

		public static Quaternion FromEulerRotation(ref this Quaternion q, Vector3 v) {
			float cy = (float) Math.Cos(v.Z * 0.5);
			float sy = (float) Math.Sin(v.Z * 0.5);
			float cp = (float) Math.Cos(v.Y * 0.5);
			float sp = (float) Math.Sin(v.Y * 0.5);
			float cr = (float) Math.Cos(v.X * 0.5);
			float sr = (float) Math.Sin(v.X * 0.5);

			q.W = (cr * cp * cy + sr * sp * sy);
			q.X = (sr * cp * cy - cr * sp * sy);
			q.Y = (cr * sp * cy + sr * cp * sy);
			q.Z = (cr * cp * sy - sr * sp * cy);

			return q;
		}

		public static Vector3 ToEulerRotation(this Quaternion q) {
			Vector3 angles = new();

			// roll / x
			double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
			double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
			angles.Z = (float) Math.Atan2(sinr_cosp, cosr_cosp);

			// pitch / y
			double sinp = 2 * (q.W * q.Y - q.Z * q.X);
			
			if(Math.Abs(sinp) >= 1) {
				angles.Y = (float) Math.CopySign(Math.PI / 2, sinp);
			} else {
				angles.Y = (float) Math.Asin(sinp);
			}

			// yaw / z
			double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
			double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
			angles.Z = (float) Math.Atan2(siny_cosp, cosy_cosp);

			return angles;
		}
	}
}