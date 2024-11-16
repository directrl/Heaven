using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.ECS;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class StaticPhysicsBody : PhysicsBody {

		public Func<TypedIndex> Shape { get; }
		
		public StaticPhysicsBody() { }

		public StaticPhysicsBody(Simulation simulation, Func<TypedIndex> shape)
			: base(simulation) {

			Shape = shape;
		}
	}
}