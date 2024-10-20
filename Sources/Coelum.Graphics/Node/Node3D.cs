using System.Numerics;

namespace Coelum.Graphics.Node {
	
	public class Node3D : SpatialNode {

		public Vector3 Position = new();
		public Vector3 Rotation = new();
		public Vector3 Scale = new(1, 1, 1);

		public Vector3 GlobalPosition {
			get {
				if(Parent is Node3D n) {
					return Position + n.GlobalPosition;
				}

				return Position;
			}
		}

		public Vector3 GlobalRotation {
			get {
				if(Parent is Node3D n) {
					return Rotation + n.GlobalRotation;
				}

				return Rotation;
			}
		}

		public Vector3 GlobalScale {
			get {
				if(Parent is Node3D n) {
					return Scale * n.GlobalScale;
				}

				return Scale;
			}
		}
		
		public override Matrix4x4 LocalTransform {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, Scale.Z);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, Position.Z);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}

		public override string ToString() {
			return $"{base.ToString()}{{Position={Position}, GlobalPosition={GlobalPosition}, Rotation={GlobalRotation}, Scale={GlobalScale}}}";
		}
	}
}