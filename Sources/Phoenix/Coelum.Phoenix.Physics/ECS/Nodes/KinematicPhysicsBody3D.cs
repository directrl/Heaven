using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class KinematicPhysicsBody3D : PhysicsBody3D {

		public override Simulation? Simulation {
			get => GetPhysicsComponent().Simulation;
			set {
				if(TryGetComponent<PhysicsBody>(out var body) && body.Simulation is not null) {
					body.Simulation.GetStore()?.RemoveBody(body);
				}
				
				AddComponent<PhysicsBody>(new KinematicPhysicsBody(value, ComputeShape));
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