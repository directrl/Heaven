using BepuPhysics;
using BepuPhysics.Collidables;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class DynamicPhysicsBody : PhysicsBody {
		
		public DynamicPhysicsBody() { }

		public DynamicPhysicsBody(Simulation simulation, Func<Shape> shape)
			: base(simulation, shape) { }
	}
}