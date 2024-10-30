using System.Numerics;

namespace Coelum.Phoenix.Component {
	
	public class Transform3D : Transform {
	
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;

		public Transform3D(Vector3? position = null,
		                   Vector3? rotation = null,
		                   Vector3? scale = null) {

			Position = position ?? new(0, 0, 0);
			Rotation = rotation ?? new(0, 0, 0);
			Scale = scale ?? new(1, 1, 1);
		}
	}
}