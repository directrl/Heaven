using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class ActiveBody3D : PhysicsBody3D {
		
		public override Simulation? Simulation {
			get => GetPhysicsComponent().Simulation;
			set {
				if(TryGetComponent<ActivePhysicsBody>(out var body) && body.Simulation is not null) {
					body.Simulation.GetStore()?.RemoveBody(body);
				}
				
				AddComponent<PhysicsBody>(new ActivePhysicsBody(value, CreateBody));
			}
		}

		public ActiveBody3D(Simulation? simulation) {
			Simulation = simulation;
		}
		
		public bool GetBody(out BodyReference reference) {
			if(Simulation is null || Simulation.GetStore() is null) {
				reference = default;
				return false;
			}

			return Simulation.GetStore().GetBody(GetComponent<ActivePhysicsBody>(), out reference);
		}
	}
}