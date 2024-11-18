using Serilog;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private Dictionary<SystemPhase, List<EcsSystem>> _systems = new();
		
		private Dictionary<SystemPhase, List<ChildQuerySystem>> _childQuerySystems = new();
		private Dictionary<SystemPhase, List<ChildQuerySystem>> _childQuerySystemsP = new();
		
		private readonly Dictionary<SystemPhase, float> _phaseDeltaTimes = new();
		
		public void AddSystem(EcsSystem system) {
			switch(system) {
				case ChildQuerySystem cqs:
					AddSpTP(ref _childQuerySystems, ref _childQuerySystemsP,
					        system.Phase, cqs, cqs.Query.Parallel);
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

			void GetFromDict<T>(Dictionary<SystemPhase, List<T>> dict)
				where T : EcsSystem {
				
				foreach(var (phase, systemList) in dict) {
					if(!systems.ContainsKey(phase)) {
						systems[phase] = new();
					}
				
					foreach(var system in systemList) {
						systems[phase].Add(system);
					}
				}
			}
			
			GetFromDict(_systems);
			GetFromDict(_childQuerySystems);
			GetFromDict(_childQuerySystemsP);

			return systems;
		}
		
		public IReadOnlyList<EcsSystem> GetSystems(SystemPhase phase) {
			var systems = new List<EcsSystem>();
			
			void GetFromDict<T>(Dictionary<SystemPhase, List<T>> dict)
				where T : EcsSystem {
				
				if(dict.ContainsKey(phase)) {
					foreach(var system in dict[phase]) {
						systems.Add(system);
					}
				}
			}

			GetFromDict(_systems);
			GetFromDict(_childQuerySystems);
			GetFromDict(_childQuerySystemsP);

			return systems;
		}
		
		public float GetDeltaTime(SystemPhase phase)
			=> _phaseDeltaTimes.GetValueOrDefault(phase, 0);
	}
}