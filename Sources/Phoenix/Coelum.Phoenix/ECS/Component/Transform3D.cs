using System.Numerics;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform3D : Transform {
	
		private Vector3 _position;
		public Vector3 Position {
			get => _position;
			set {
				Dirty = true;
				_position = value;
			}
		}

		private Vector3 _rotation;
		public Vector3 Rotation {
			get => _rotation;
			set {
				Dirty = true;
				_rotation = value;
			}
		}

		private Vector3 _scale;
		public Vector3 Scale {
			get => _scale;
			set {
				Dirty = true;
				_scale = value;
			}
		}
		
		public Vector3 GlobalPosition { get; internal set; }
		public Vector3 GlobalRotation { get; internal set; }
		public Vector3 GlobalScale { get; internal set; }

		public Transform3D(Vector3? position = null,
		                   Vector3? rotation = null,
		                   Vector3? scale = null) {

			Position = position ?? new(0, 0, 0);
			Rotation = rotation ?? new(0, 0, 0);
			Scale = scale ?? new(1, 1, 1);
		}
	}
}