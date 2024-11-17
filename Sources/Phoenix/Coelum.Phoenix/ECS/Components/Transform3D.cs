using System.Numerics;
using Coelum.LanguageExtensions;
using Hexa.NET.ImGuizmo;

namespace Coelum.Phoenix.ECS.Components {
	
	public class Transform3D : Transform {
	
		public Vector3 Position = new(0, 0, 0);
		public Vector3 Rotation = new(0, 0, 0);
		public Vector3 Scale = new(1, 1, 1);
		public Vector3 Offset = Vector3.Zero;

		public float Yaw {
			get => Rotation.Y.ToDegrees();
			set => Rotation.Y = value.ToRadians();
		}

		public float Pitch {
			get => Rotation.X.ToDegrees();
			set => Rotation.X = value.ToRadians();
		}

		public float Roll {
			get => Rotation.Z.ToDegrees();
			set => Rotation.Z = value.ToRadians();
		}

		public Vector3 GlobalPosition {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform3D>(out var pt)) {
					return Position + pt.GlobalPosition;
				}

				return Position;
			}
		}
		
		public Vector3 GlobalRotation {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform3D>(out var pt)) {
					return Rotation + pt.GlobalRotation;
				}

				return Rotation;
			}
		}
		
		public Vector3 GlobalScale {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform3D>(out var pt)) {
					return Scale * pt.GlobalScale;
				}

				return Scale;
			}
		}
		
		public Quaternion QGlobalRotation {
			get {
				/*var qX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, GlobalRotation.X);
				var qY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, GlobalRotation.Y);
				var qZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, GlobalRotation.Z);

				return Quaternion.Normalize(qY * qX * qZ);*/
				
				float cy = (float) Math.Cos(GlobalRotation.Z * 0.5);
				float sy = (float) Math.Sin(GlobalRotation.Z * 0.5);
				float cp = (float) Math.Cos(GlobalRotation.Y * 0.5);
				float sp = (float) Math.Sin(GlobalRotation.Y * 0.5);
				float cr = (float) Math.Cos(GlobalRotation.X * 0.5);
				float sr = (float) Math.Sin(GlobalRotation.X * 0.5);

				return new() {
					W = (cr * cp * cy + sr * sp * sy),
					X = (sr * cp * cy - cr * sp * sy),
					Y = (cr * sp * cy + sr * cp * sy),
					Z = (cr * cp * sy - sr * sp * cy)
				};
			}
		}

		public float GlobalYaw => GlobalRotation.Y;
		public float GlobalPitch => GlobalRotation.X;
		public float GlobalRoll => GlobalRotation.Z;

		public Transform3D() { }
		
		public Transform3D(Vector3? position = null,
		                   Vector3? rotation = null,
		                   Vector3? scale = null) {

			if(position.HasValue) Position = position.Value;
			if(rotation.HasValue) Rotation = rotation.Value;
			if(scale.HasValue) Scale = scale.Value;
		}
		
		public void UpdateFromComponents(Vector3 position, Quaternion orientation, Vector3 scale) {
			var matrix = Matrix4x4.CreateScale(scale)
				* Matrix4x4.CreateFromQuaternion(orientation)
				* Matrix4x4.CreateTranslation(position);

			UpdateFromMatrix(matrix);
		}
		
		public void UpdateFromMatrix(Matrix4x4 matrix) {
			var translationMatrix = new Matrix4x4();
			var rotationMatrix = new Matrix4x4();
			var scaleMatrix = new Matrix4x4();
			
			ImGuizmo.DecomposeMatrixToComponents(
				ref matrix,
				ref translationMatrix,
				ref rotationMatrix,
				ref scaleMatrix
			);
			
			var resultTranslation = new Vector3(translationMatrix.M11, translationMatrix.M12, translationMatrix.M13);
			var resultRotation = new Vector3(rotationMatrix.M11.ToRadians(), rotationMatrix.M12.ToRadians(), rotationMatrix.M13.ToRadians());
			var resultScale = new Vector3(scaleMatrix.M11, scaleMatrix.M12, scaleMatrix.M13);

			Position = resultTranslation;
			Rotation = resultRotation;
			Scale = resultScale;
		}
	}
}