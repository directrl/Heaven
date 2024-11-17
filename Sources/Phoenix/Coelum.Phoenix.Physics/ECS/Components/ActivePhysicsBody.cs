using BepuPhysics;
using BepuPhysics.Collidables;

namespace Coelum.Phoenix.Physics.ECS.Components {
	
	public class ActivePhysicsBody : PhysicsBody {
		
		public ActivePhysicsBody() { }

		public ActivePhysicsBody(Simulation? simulation, Func<Data> shape)
			: base(simulation, shape) { }
	}
}