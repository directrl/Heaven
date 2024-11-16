using BepuPhysics;
using BepuPhysics.Collidables;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class KinematicPhysicsBody : PhysicsBody {
		
		public Func<TypedIndex> Shape { get; }
		public bool Convex { get; }
		
		public KinematicPhysicsBody() { }

		public KinematicPhysicsBody(Simulation simulation, Func<TypedIndex> shape)
			: base(simulation) {

			Shape = shape;
		}
	}
}