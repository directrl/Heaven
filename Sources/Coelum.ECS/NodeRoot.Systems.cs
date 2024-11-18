using Serilog;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private Dictionary<SystemPhase, List<EcsSystem>> _systems = new();
		private Dictionary<SystemPhase, List<ChildQuerySystem>> _childQuerySystems = new();
		
		private readonly Dictionary<SystemPhase, float> _phaseDeltaTimes = new();
		
		public void AddSystem(EcsSystem system) {
			switch(system) {
				case ChildQuerySystem cqs:
					if(!_childQuerySystems.ContainsKey(system.Phase)) {
						_childQuerySystems[system.Phase] = new();
					}
					
					_childQuerySystems[system.Phase].Add(cqs);
					break;
				default:
					if(!_systems.ContainsKey(system.Phase)) {
						_systems[system.Phase] = new();
					}

					_systems[system.Phase].Add(system);
					break;
			}
			
			Log.Debug($"[ECS] New {system.GetType().Name} system registered for phase {system.Phase}");
		}

		public IReadOnlyDictionary<SystemPhase, List<EcsSystem>> GetSystems() {
			var systems = new Dictionary<SystemPhase, List<EcsSystem>>();

			foreach(var (phase, systemList) in _systems) {
				systems[phase] = new();
				
				foreach(var system in systemList) {
					systems[phase].Add(system);
				}
			}
			
			foreach(var (phase, systemList) in _childQuerySystems) {
				systems[phase] = new();
				
				foreach(var system in systemList) {
					systems[phase].Add(system);
				}
			}

			return systems;
		}
		
		public IReadOnlyList<EcsSystem> GetSystems(SystemPhase phase) {
			var systems = new List<EcsSystem>();

			if(_systems.ContainsKey(phase)) {
				foreach(var system in _systems[phase]) {
					systems.Add(system);
				}
			}

			if(_childQuerySystems.ContainsKey(phase)) {
				foreach(var system in _childQuerySystems[phase]) {
					systems.Add(system);
				}
			}

			return systems;
		}
		
		public float GetDeltaTime(SystemPhase phase)
			=> _phaseDeltaTimes.GetValueOrDefault(phase, 0);
	}
}