using BepuPhysics;

namespace Coelum.Phoenix.Physics {
	
	public static class SimulationExtensions {
		
		public static PhysicsStore? GetStore(this Simulation simulation) {
			if(SimulationManager.GetPhysicsStore(simulation, out var store)) {
				return store;
			}

			return null;
		}

		public static int? GetId(this Simulation simulation) {
			if(SimulationManager.GetId(simulation, out var id)) {
				return id;
			}

			return null;
		}
	}
}