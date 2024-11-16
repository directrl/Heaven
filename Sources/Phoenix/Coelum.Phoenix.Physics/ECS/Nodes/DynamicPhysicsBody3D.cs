using BepuPhysics;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class DynamicPhysicsBody3D : PhysicsBody3D {

		public DynamicPhysicsBody3D(Simulation? simulation) : base(simulation) {
			if(simulation is not null) {
				AddComponent<PhysicsBody>(new DynamicPhysicsBody(simulation, ComputeShape));
			}
		}
		
		public bool GetBody(out BodyReference reference) {
			if(Simulation is null) {
				reference = default;
				return false;
			}

			return Simulation.GetStore().GetBody(GetComponent<PhysicsBody>(), out reference);
		}
	}
}