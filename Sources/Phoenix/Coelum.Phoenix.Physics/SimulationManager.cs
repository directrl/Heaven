using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Coelum.Common.Graphics;

namespace Coelum.Phoenix.Physics {
	
	public static class SimulationManager {
		
		private static readonly ConcurrentDictionary<int, (Simulation, ThreadDispatcher)> _SIMULATIONS = new();
		private static readonly ConcurrentDictionary<SceneBase, ConcurrentBag<int>> _SCENE_SIMULATIONS = new();
		private static readonly ConcurrentDictionary<Simulation, PhysicsStore> _PHYSICS_STORES = new();

		private static int _lastId = 0;
		
		public static BufferPool? BufferPool { get; internal set; }

		public static IReadOnlyDictionary<int, (Simulation, ThreadDispatcher)> Simulations {
			get => new ReadOnlyDictionary<int, (Simulation, ThreadDispatcher)>(_SIMULATIONS);
		}

		public static int AddSimulation(SceneBase scene, Simulation simulation, ThreadDispatcher dispatcher) {
			if(!_SIMULATIONS.TryAdd(++_lastId, (simulation, dispatcher))) {
				throw new Exception("Could not add simulation to dictionary");
			}

			if(!_PHYSICS_STORES.TryAdd(simulation, new(simulation))) {
				throw new Exception("Could not add physics store to dictionary");
			}

			var sceneSimulations = _SCENE_SIMULATIONS.GetOrAdd(scene, new ConcurrentBag<int>());
			sceneSimulations.Add(_lastId);
			
			return _lastId;
		}

		public static List<Simulation> GetSimulationsByScene(SceneBase scene) {
			var sceneSimulations = new List<Simulation>();

			foreach(var id in _SCENE_SIMULATIONS.GetOrAdd(scene, new ConcurrentBag<int>())) {
				if(GetById(id, out var simulation)) {
					sceneSimulations.Add(simulation.Simulation);
				}
			}

			return sceneSimulations;
		}

		public static bool GetById(int id, out (Simulation Simulation, ThreadDispatcher Dispatcher) value) {
			return _SIMULATIONS.TryGetValue(id, out value);
		}

		public static bool GetId(Simulation simulation, out int id) {
			foreach(var (k, v) in _SIMULATIONS) {
				if(v.Item1 == simulation) {
					id = k;
					return true;
				}
			}

			id = default;
			return false;
		}

		public static bool GetPhysicsStore(Simulation simulation, out PhysicsStore store) {
			return _PHYSICS_STORES.TryGetValue(simulation, out store);
		}
	}
}