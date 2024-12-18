using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.ECS;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class StaticPhysicsBody : PhysicsBody {
		
		public StaticPhysicsBody() { }

		public StaticPhysicsBody(Simulation? simulation, Func<Data> shape)
			: base(simulation, shape) { }
	}
}