using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class Node2D : Node {
		
		public Transform2D Transform { get; }

		public Vector2 Position {
			get => Transform.Position;
			set => Transform.Position = value;
		}
		
		public float Rotation {
			get => Transform.Rotation;
			set => Transform.Rotation = value;
		}
		
		public Vector2 Scale {
			get => Transform.Scale;
			set => Transform.Scale = value;
		}

		public Node2D() {
			Transform = new();
			AddComponent<Transform>(Transform);
		}
	}
}