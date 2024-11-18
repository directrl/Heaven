namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		public Query<SystemPhase, List<EcsSystem>> QuerySystems() {
			return new(
				each => {
					foreach((var phase, var systems) in _systems) {
						each?.Invoke(phase, systems);
					}
				}
			);
		}

		public T? QuerySystem<T>(SystemPhase? phase = null) where T : EcsSystem {
			if(phase != null) {
				foreach(var system in _systems[phase.Value]) {
					if(system is T t) return t;
				}
			} else {
				foreach(var (_, systems) in _systems) {
					foreach(var system in systems) {
						if(system is T t) return t;
					}
				}
			}

			return null;
		}
	}
}