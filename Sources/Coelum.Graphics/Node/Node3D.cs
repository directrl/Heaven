using System.Numerics;

namespace Coelum.Graphics.Node {
	
	public class Node3D : SpatialNode {

		public Vector3 Position = new();
		public Vector3 Rotation = new();
		public Vector3 Scale = new(1, 1, 1);
		
		public override Matrix4x4 ModelMatrix {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, Position.Z);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}

		public override string ToString() {
			return $"{base.ToString()}{{Position={Position}, Rotation={Rotation}, Scale={Scale}}}";
		}
	}
}