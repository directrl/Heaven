using System.Numerics;
using Coelum.Common.Ecs;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs.Component {
	
	public class Transform2D : Transform {

		public Vector2 Position;
		public float Rotation;
		public Vector2 Scale;
		
		public Transform2D(Vector2? position = null,
		                   float rotation = 0,
		                   Vector2? scale = null) {

			Position = position ?? new(0, 0);
			Rotation = rotation;
			Scale = scale ?? new(1, 1);
		}

		public static Vector2 GlobalPosition(Entity e) {
			return e.GetGlobal(
				(Transform2D t) => t.Position,
				(arg1, arg2) => arg1 + arg2
			);
		}
		
		public static float GlobalRotation(Entity e) {
			return e.GetGlobal(
				(Transform2D t) => t.Rotation,
				(arg1, arg2) => arg1 + arg2
			);
		}
		
		public static Vector2 GlobalScale(Entity e) {
			return e.GetGlobal(
				(Transform2D t) => t.Scale,
				(arg1, arg2) => arg1 * arg2
			);
		}
	}
}