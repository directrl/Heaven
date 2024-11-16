using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.ECS;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public abstract class PhysicsBody : INodeComponent {

		public Node? Owner { get; set; }
		
		public Simulation Simulation { get; set; }
		public Func<Shape> ComputeShape { get; }
		
		public bool DoUpdates { get; set; } = true;
		
		[Obsolete] // TODO Obsolete
		public bool Created { get; internal set; }
		
		public PhysicsBody() { }
		
		public PhysicsBody(Simulation simulation, Func<Shape> shape) {
			Simulation = simulation;
			ComputeShape = shape;
		}

		public struct Shape {

			public TypedIndex Index;
			public float Mass;
			public BodyInertia Inertia;
		}
	}
}