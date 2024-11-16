using BepuPhysics;
using BepuPhysics.Collidables;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class KinematicPhysicsBody : PhysicsBody {
		
		public KinematicPhysicsBody() { }

		public KinematicPhysicsBody(Simulation simulation, Func<Shape> shape)
			: base(simulation, shape) { }
	}
}