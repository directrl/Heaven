using System.Numerics;

namespace Coelum.Phoenix.Node {
	
	public class Node2D : SpatialNode {

		public Vector2 Position = new();
		public float Rotation = 0;
		public Vector2 Scale = new(1, 1);
		
		public override Matrix4x4 LocalTransform {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, 1);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, 1);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(1, 1, Rotation);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}
		
		public override string ToString() {
			return $"{base.ToString()}{{Position={Position}, Rotation={Rotation}, Scale={Scale}}}";
		}
	}
}