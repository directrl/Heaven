using System.Numerics;

namespace Coelum.Phoenix.Component {
	
	public class Transform2D : Transform {
		
		public Vector2 Position;
		public float Rotation;
		public Vector2 Scale;

		public Transform2D(Vector2? position = null,
		                   float rotation = 0,
		                   Vector2? scale = null) {

			Position = position ?? new(0, 0);
			Rotation = rotation;
			Scale = scale ?? new(1, 1);
		}
	}
}