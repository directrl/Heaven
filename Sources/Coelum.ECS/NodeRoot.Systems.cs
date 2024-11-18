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
		
		public void RemoveSystem(Type system) {
			RunLater(() => {
			#region Regular systems
				foreach(var systems in _systems.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
			#endregion

			#region Child query systems
				foreach(var systems in _childQuerySystems.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
				
				foreach(var systems in _childQuerySystemsP.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
			#endregion
			});
		}

		public void ReplaceSystem(Type what, EcsSystem with) {
			RunLater(() => {
			#region Regular systems
				foreach(var systems in _systems.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = with;
					}
				}
			#endregion

			#region Child query systems
				foreach(var systems in _childQuerySystems.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = (ChildQuerySystem) with;
					}
				}
				
				foreach(var systems in _childQuerySystemsP.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = (ChildQuerySystem) with;
					}
				}
			#endregion
			});
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