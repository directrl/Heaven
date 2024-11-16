using BepuPhysics;
using BepuPhysics.Collidables;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class DynamicPhysicsBody : PhysicsBody {
		
		public Func<(TypedIndex, BodyInertia)> Shape { get; }
		
		public DynamicPhysicsBody() { }

		public DynamicPhysicsBody(Simulation simulation, Func<(TypedIndex, BodyInertia)> shape)
			: base(simulation) {

			Shape = shape;
		}
	}
}