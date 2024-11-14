using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform2D : Transform {

		public Node? Owner { get; set; }
		
		public Matrix4x4 LocalMatrix { get; set; }
		public Matrix4x4 GlobalMatrix { get; set; }
		
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

		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(GetType().FullName);
			writer.WriteString("backing_type", name);
			{
				Position.Serializer().Serialize("position", writer);
				writer.WriteNumber("rotation", Rotation);
				Scale.Serializer().Serialize("scale", writer);
			}
			writer.WriteEndObject();
		}
		
		public INodeComponent Deserialize(JsonNode node) {
			Position = new Vector2Serializer().Deserialize(node["position"]);
			Rotation = node["rotation"].GetValue<float>();
			Scale = new Vector2Serializer().Deserialize(node["scale"]);
			
			return this;
		}
	}
}