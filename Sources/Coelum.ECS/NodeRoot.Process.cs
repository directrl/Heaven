namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		private void SystemProcess(SystemPhase phase) {
			_childQueries.TryGetValue(phase, out var q);
			_childQuerySystems.TryGetValue(phase, out var qs);

			if(qs is not null) {
				foreach(var system in qs) {
					system.Reset();
				}
			}

			if(q is not null || qs is not null) {
				foreach(var node in _nodes.Values) {
					if(q is not null) {
						foreach(var query in q) {
							query.Call(this, node);
						}
					}

					if(qs is not null) {
						foreach(var system in qs) {
							system.Invoke(this, node);
						}
					}
				}
			}
		}
		
		private void ParallelSystemProcess(SystemPhase phase) {
		#region Child queries
			_childQueriesP.TryGetValue(phase, out var q);
			_childQuerySystemsP.TryGetValue(phase, out var qs);

			if(qs is not null) {
				foreach(var system in qs) {
					system.Reset();
				}
			}

			if(q is not null) {
				Parallel.ForEach(_nodes.Values, new ParallelOptions() {
					MaxDegreeOfParallelism = (int) Math.Ceiling(Environment.ProcessorCount / 3d)
				}, node => {
					foreach(var query in q) {
						query.Call(this, node);
					}
				});
			}
			
			if(qs is not null) {
				Parallel.ForEach(_nodes.Values, new ParallelOptions() {
					MaxDegreeOfParallelism = (int) Math.Ceiling(Environment.ProcessorCount / 3d)
				}, node => {
					foreach(var system in qs) {
						system.Invoke(this, node);
					}
				});
			}
		#endregion
		}
	}
}