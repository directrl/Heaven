using BepuPhysics;

namespace Coelum.Phoenix.Physics {
	
	public static class SimulationExtensions {
		
		internal static uint _lastSimulationId = 0;
		internal static Dictionary<uint, Simulation> _simulations = new();

		private static Dictionary<Simulation, PhysicsStore> _stores = new();
		
		public static PhysicsStore GetStore(this Simulation simulation) {
			if(!_stores.TryGetValue(simulation, out var store)) {
				store = new(simulation);
				_stores[simulation] = store;
			}

			return store;
		}

		public static Simulation? GetSimulationById(uint id) {
			if(_simulations.TryGetValue(id, out var simulation)) {
				return simulation;
			}

			return null;
		}

		public static uint? GetSimulationId(Simulation simulation) {
			if(!_simulations.ContainsValue(simulation)) return null;
			return _simulations.First(kv => kv.Value == simulation).Key;
		}
	}
}