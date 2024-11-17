using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class KinematicPhysicsBody3D : PhysicsBody3D {

		public override Simulation? Simulation {
			get => GetPhysicsComponent().Simulation;
			set {
				if(value is not null) {
					AddComponent<PhysicsBody>(new DynamicPhysicsBody(value, ComputeShape));
				}
			}
		}

		public KinematicPhysicsBody3D(Simulation? simulation) {
			Simulation = simulation;
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