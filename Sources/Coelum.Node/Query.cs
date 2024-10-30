namespace Coelum.Node {
	
	public class Query<T> {

		private Action<Action<T>?> _query;
		private Action<Action<T>?>? _parallelQuery;

		private bool _parallel;

		private Action<T>? _each;
		
		public Query(Action<Action<T>?> query, Action<Action<T>?>? parallelQuery = null) {
			_query = query;
			_parallelQuery = parallelQuery;
		}

		public Query<T> Each(Action<T> action) {
			_each = action;
			return this;
		}

		public Query<T> Parallel(bool parallel) {
			_parallel = parallel;
			return this;
		}

		public void Execute() {
			if(_parallel && _parallelQuery != null) _parallelQuery.Invoke(_each);
			else _query.Invoke(_each);
		}
	}
}