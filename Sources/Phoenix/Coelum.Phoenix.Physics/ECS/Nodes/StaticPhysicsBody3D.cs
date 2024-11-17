using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class StaticPhysicsBody3D : PhysicsBody3D {
		
		public override Simulation? Simulation {
			get => GetPhysicsComponent().Simulation;
			set {
				if(value is not null) {
					AddComponent<PhysicsBody>(new StaticPhysicsBody(value, ComputeShape));
				}
			}
		}

		public StaticPhysicsBody3D(Simulation? simulation) {
			Simulation = simulation;
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