using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class StaticPhysicsBody3D : PhysicsBody3D {

		public StaticPhysicsBody3D(Simulation? simulation) : base(simulation) {
			if(simulation is not null) {
				AddComponent<PhysicsBody>(new StaticPhysicsBody(simulation, ComputeShape));
			}
		}

		public bool GetStatic(out StaticReference reference) {
			if(Simulation is null) {
				reference = default;
				return false;
			}

			return Simulation.GetStore().GetStatic(GetComponent<StaticPhysicsBody>(), out reference);
		}
	}
}