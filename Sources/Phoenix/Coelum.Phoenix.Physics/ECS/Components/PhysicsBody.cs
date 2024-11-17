using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.ECS;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public abstract class PhysicsBody : INodeComponent {

		public Node? Owner { get; set; }
		
		public Simulation? Simulation { get; set; }
		public Func<Data> CreateData { get; }

		private bool _doUpdates = true;
		public bool DoUpdates {
			get => _doUpdates;
			set {
				if(!_doUpdates && value) Dirty = true;
				_doUpdates = value;
			}
		}
		
		public bool Dirty { get; set; }
		
		public PhysicsBody() { }
		
		public PhysicsBody(Simulation? simulation, Func<Data> shape) {
			Simulation = simulation;
			CreateData = shape;
		}

		public struct Data {

			public TypedIndex Index;
			public float Mass;
			public BodyInertia Inertia;
		}
	}
}