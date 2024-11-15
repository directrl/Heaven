using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform2D : Transform {
		
		public Vector2 Position;
		public float Rotation;
		public Vector2 Scale;
		
		public Vector2 GlobalPosition {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform2D>(out var pt)) {
					return Position + pt.GlobalPosition;
				}

				return Position;
			}
		}
		
		public float GlobalRotation {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform2D>(out var pt)) {
					return Rotation + pt.GlobalRotation;
				}

				return Rotation;
			}
		}
		
		public Vector2 GlobalScale {
			get {
				if(Owner is { Parent: not null } && Owner.Parent
				                                         .TryGetComponent<Transform, Transform2D>(out var pt)) {
					return Scale * pt.GlobalScale;
				}

				return Scale;
			}
		}
		
		public Transform2D() { }

		public Transform2D(Vector2? position = null,
		                   float rotation = 0,
		                   Vector2? scale = null) {

			Position = position ?? new(0, 0);
			Rotation = rotation;
			Scale = scale ?? new(1, 1);
		}
	}
}