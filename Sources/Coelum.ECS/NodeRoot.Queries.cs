using Coelum.ECS.Queries;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private Dictionary<SystemPhase, List<IChildQuery>> _childQueries = new();
		private Dictionary<SystemPhase, List<IChildQuery>> _childQueriesP = new();
		
		public void AddQuery(IChildQuery query)
			=> AddSpTP(ref _childQueries, ref _childQueriesP,
			           query.Phase, query, query.Parallel);
	}
}