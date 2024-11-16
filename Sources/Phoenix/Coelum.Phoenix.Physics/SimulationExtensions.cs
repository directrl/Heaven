using BepuPhysics;

namespace Coelum.Phoenix.Physics {
	
	public static class SimulationExtensions {

		private static Dictionary<Simulation, PhysicsStore> _stores = new();
		
		public static PhysicsStore GetStore(this Simulation simulation) {
			if(!_stores.TryGetValue(simulation, out var store)) {
				store = new(simulation);
				_stores[simulation] = store;
			}

			return store;
		}
	}
}