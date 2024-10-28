using System.Numerics;

namespace Coelum.World.Components {

	public record Position3D(float X, float Y, float Z) {

		public Vector3 Vector => new(X, Y, Z);
	}
}