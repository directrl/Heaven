using Coelum.ECS.Queries;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private Dictionary<SystemPhase, List<IChildQuery>> _childQueries = new();

		public void AddQuery(IChildQuery query) {
			if(!_childQueries.ContainsKey(query.Phase)) {
				_childQueries[query.Phase] = new();
			}
			
			_childQueries[query.Phase].Add(query);
		}
	}
}