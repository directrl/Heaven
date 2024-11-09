namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		public Query<string, List<EcsSystem>> QuerySystems() {
			return new(
				each => {
					foreach((var phase, var systems) in _systems) {
						each?.Invoke(phase, systems);
					}
				}
			);
		}

		public T? QuerySystem<T>(string phase) where T : EcsSystem {
			foreach(var system in _systems[phase]) {
				if(system is T t) return t;
			}

			return null;
		}
	}
}