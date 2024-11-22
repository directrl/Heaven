using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class Node3D : Node {

		public Transform3D Transform { get; }

		public Vector3 Position {
			get => Transform.Position;
			set => Transform.Position = value;
		}
		
		public Vector3 Rotation {
			get => Transform.Rotation;
			set => Transform.Rotation = value;
		}
		
		public Vector3 Scale {
			get => Transform.Scale;
			set => Transform.Scale = value;
		}
		
		public Node3D() {
			Transform = new();
			AddComponent<Transform>(Transform);
		}
	}
}