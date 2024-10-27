using Silk.NET.Maths;

namespace Coelum.Physics.Collision {
	
	public class BoundingBox2D<T> : ICollidable<BoundingBox2D<T>>, ICollidable<Vector2D<T>>
		where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T> {

		public Vector2D<T> Min { get; }
		public Vector2D<T> Max { get; }

		public BoundingBox2D(Vector2D<T> min, Vector2D<T> max) {
			Min = min;
			Max = max;
		}
		
		public bool Collides(in BoundingBox2D<T> other) {
			/*
			 *   +----x
			 *   |    |
			 * +----x |
			 * | n--|-+
			 * |    |
			 * n----+
			 */
			
			return Scalar.GreaterThanOrEqual<T>(other.Min.X, Min.X)
				&& Scalar.GreaterThanOrEqual<T>(other.Min.Y, Min.Y)
				&& Scalar.LessThanOrEqual<T>(other.Max.X, Max.X)
				&& Scalar.LessThanOrEqual<T>(other.Max.Y, Max.Y);
		}

		public bool Collides(in Vector2D<T> other) {
			return Scalar.GreaterThanOrEqual<T>(other.X, Min.X)
				&& Scalar.GreaterThanOrEqual<T>(other.Y, Min.Y)
				&& Scalar.LessThanOrEqual<T>(other.X, Max.X)
				&& Scalar.LessThanOrEqual<T>(other.Y, Max.Y);
		}
	}
}