namespace Coelum.LanguageExtensions {

	public static class MathExtensions {

		public static double ToRadians(this double degrees) {
			return (Math.PI / 180) * degrees;
		}

		public static float ToRadians(this float degrees) {
			return (MathF.PI / 180) * degrees;
		}
		
		public static double ToDegrees(this double radians) {
			return (180 / Math.PI) * radians;
		}

		public static float ToDegrees(this float radians) {
			return (180 / MathF.PI) * radians;
		}
	}
}