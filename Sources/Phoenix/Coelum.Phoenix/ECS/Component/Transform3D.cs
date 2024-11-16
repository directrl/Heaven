using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.Phoenix.ECS.Component {
	
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
			get => Rotation.Z.ToDegrees();
			set => Rotation.Z = value.ToRadians();
		}

		public float Roll {
			get => Rotation.X.ToDegrees();
			set => Rotation.X = value.ToRadians();
		}

		public Quaternion QRotation {
			get {
				var qX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, Rotation.X);
				var qY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, Rotation.Y);
				var qZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, Rotation.Z);

				return qX * qY * qZ;
			}
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
				var qX = Quaternion.CreateFromAxisAngle(Vector3.UnitX, GlobalRotation.X);
				var qY = Quaternion.CreateFromAxisAngle(Vector3.UnitY, GlobalRotation.Y);
				var qZ = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, GlobalRotation.Z);

				return qX * qY * qZ;
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
		
		public void SetLocalFromGlobal(Vector3? position = null,
		                               Vector3? rotation = null,
									   Vector3? scale = null) {

			position ??= GlobalPosition;
			rotation ??= GlobalRotation;
			scale ??= GlobalScale;

			var newMatrix = new Matrix4x4();

			if(Owner.Parent != null
			   && Owner.Parent.TryGetComponent<Transform3D>(out var pt)) {
				Matrix4x4.Invert(pt.GlobalMatrix, out var parentGlobalMatrix);
				
				
			}
		}
	}
}