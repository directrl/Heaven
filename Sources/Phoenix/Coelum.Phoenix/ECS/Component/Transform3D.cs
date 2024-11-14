using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.LanguageExtensions.Serialization;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform3D : Transform {
		
		public Node? Owner { get; set; }
		
		public Matrix4x4 LocalMatrix { get; set; }
		public Matrix4x4 GlobalMatrix { get; set; }
	
		public Vector3 Position;
		public Vector3 Rotation;
		public Vector3 Scale;

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

		public float GlobalYaw => GlobalRotation.Y;
		public float GlobalPitch => GlobalRotation.X;
		public float GlobalRoll => GlobalRotation.Z;

		public Transform3D() { }
		
		public Transform3D(Vector3? position = null,
		                   Vector3? rotation = null,
		                   Vector3? scale = null) {

			Position = position ?? new(0, 0, 0);
			Rotation = rotation ?? new(0, 0, 0);
			Scale = scale ?? new(1, 1, 1);
		}

		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(GetType().FullName);
			writer.WriteString("backing_type", name);
			{
				Position.Serializer().Serialize("position", writer);
				Rotation.Serializer().Serialize("rotation", writer);
				Scale.Serializer().Serialize("scale", writer);
			}
			writer.WriteEndObject();
		}
		
		public INodeComponent Deserialize(JsonNode node) {
			Position = new Vector3Serializer().Deserialize(node["position"]);
			Rotation = new Vector3Serializer().Deserialize(node["rotation"]);
			Scale = new Vector3Serializer().Deserialize(node["scale"]);
			
			return this;
		}
	}
}