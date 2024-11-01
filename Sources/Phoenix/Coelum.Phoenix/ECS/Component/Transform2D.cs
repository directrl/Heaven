using System.Numerics;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform2D : Transform {

		private Vector2 _position;
		public Vector2 Position {
			get => _position;
			set {
				Dirty = true;
				_position = value;
			}
		}

		private float _rotation;
		public float Rotation {
			get => _rotation;
			set {
				Dirty = true;
				_rotation = value;
			}
		}

		private Vector2 _scale;
		public Vector2 Scale {
			get => _scale;
			set {
				Dirty = true;
				_scale = value;
			}
		}
		
		public Vector2 GlobalPosition { get; internal set; }
		public float GlobalRotation { get; internal set; }
		public Vector2 GlobalScale { get; internal set; }

		public Transform2D(Vector2? position = null,
		                   float rotation = 0,
		                   Vector2? scale = null) {

			Position = position ?? new(0, 0);
			Rotation = rotation;
			Scale = scale ?? new(1, 1);
		}
	}
}