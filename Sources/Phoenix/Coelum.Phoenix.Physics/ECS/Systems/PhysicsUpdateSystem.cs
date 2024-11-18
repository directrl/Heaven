using BepuPhysics;
using BepuUtilities;
using Coelum.ECS;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class PhysicsUpdateSystem : EcsSystem {

		public override string Name => "Physics Timestep";
		public override SystemPhase Phase => SystemPhase.FIXED_UPDATE;

		public Simulation Simulation { get; set; }
		public ThreadDispatcher ThreadDispatcher { get; set; }
		
		public PhysicsUpdateSystem(Simulation simulation, ThreadDispatcher dispatcher) {
			Simulation = simulation;
			ThreadDispatcher = dispatcher;
			
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			Simulation.Timestep(delta, ThreadDispatcher);
		}
	}
}