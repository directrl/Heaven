using System.Numerics;

namespace Coelum.LanguageExtensions {
	
	public static class QuaternionExtensions {

		public static Vector3 ToPitchYawRoll(this Quaternion q) {
			float yaw = MathF.Atan2(2.0f * (q.W * q.Y + q.X + q.Z), 1.0f - 2.0f * (MathF.Pow(q.Y, 2) + MathF.Pow(q.Z, 2)));
			float pitch = MathF.Asin(2.0f * (q.W * q.X - q.Y * q.Z));
			float roll = MathF.Atan2(2.0f * (q.W * q.Z + q.X * q.Y), 1.0f - 2.0f * (MathF.Pow(q.X, 2) + MathF.Pow(q.Z, 2)));
			return new(pitch, yaw, roll);
		}
	}
}