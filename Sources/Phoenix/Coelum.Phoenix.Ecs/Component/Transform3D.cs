using System.Numerics;

namespace Coelum.Phoenix.Ecs.Component {
	
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

		public override Matrix4x4 Matrix {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, Position.Z);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}
	}
}