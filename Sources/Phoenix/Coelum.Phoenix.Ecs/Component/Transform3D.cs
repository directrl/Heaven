using System.Numerics;
using Coelum.Common.Ecs;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs.Component {
	
	public class Transform3D : Transform {

		private Vector3 _position;
		public Vector3 Position {
			get => _position;
			set {
				_position = value;
				Dirty = true;
			}
		}
		
		private Vector3 _rotation;
		public Vector3 Rotation {
			get => _rotation;
			set {
				_rotation = value;
				Dirty = true;
			}
		}
		
		private Vector3 _scale;
		public Vector3 Scale {
			get => _scale;
			set {
				_scale = value;
				Dirty = true;
			}
		}

		public Transform3D(Vector3? position = null,
		                   Vector3? rotation = null,
		                   Vector3? scale = null) {

			Position = position ?? new(0, 0, 0);
			Rotation = rotation ?? new(0, 0, 0);
			Scale = scale ?? new(1, 1, 1);
		}
		
		public static Vector3 GlobalPosition(Entity e) {
			return e.GetGlobal(
				(Transform3D t) => t.Position,
				(arg1, arg2) => arg1 + arg2
			);
		}
		
		public static Vector3 GlobalRotation(Entity e) {
			return e.GetGlobal(
				(Transform3D t) => t.Rotation,
				(arg1, arg2) => arg1 + arg2
			);
		}
		
		public static Vector3 GlobalScale(Entity e) {
			return e.GetGlobal(
				(Transform3D t) => t.Scale,
				(arg1, arg2) => arg1 * arg2
			);
		}
	}
}