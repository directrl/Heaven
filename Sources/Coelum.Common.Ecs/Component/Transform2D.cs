using System.Numerics;

namespace Coelum.Common.Ecs.Component {
	
	public class Transform2D : Transform {

		public Vector2 Position = new(0, 0);
		public float Rotation = 0;
		public Vector2 Scale = new(1, 1);
		
		public Transform2D(Vector2? position = null,
		                   float rotation = 0,
		                   Vector2? scale = null) {

			Position = position ?? new(0, 0);
			Rotation = rotation;
			Scale = scale ?? new(1, 1);
		}

		public override Matrix4x4 Matrix {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, 1);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, 1);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(1, 1, Rotation);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}
	}
}