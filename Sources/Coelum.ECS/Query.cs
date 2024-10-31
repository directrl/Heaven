namespace Coelum.ECS {
	
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
	
	public class Query<T1, T2> {

		private Action<Action<T1, T2>?> _query;
		private Action<Action<T1, T2>?>? _parallelQuery;

		private bool _parallel;

		private Action<T1, T2>? _each;
		
		public Query(Action<Action<T1, T2>?> query, Action<Action<T1, T2>?>? parallelQuery = null) {
			_query = query;
			_parallelQuery = parallelQuery;
		}

		public Query<T1, T2> Each(Action<T1, T2> action) {
			_each = action;
			return this;
		}

		public Query<T1, T2> Parallel(bool parallel) {
			_parallel = parallel;
			return this;
		}

		public void Execute() {
			if(_parallel && _parallelQuery != null) _parallelQuery.Invoke(_each);
			else _query.Invoke(_each);
		}
	}
	
	public class Query<T1, T2, T3> {

		private Action<Action<T1, T2, T3>?> _query;
		private Action<Action<T1, T2, T3>?>? _parallelQuery;

		private bool _parallel;

		private Action<T1, T2, T3>? _each;
		
		public Query(Action<Action<T1, T2, T3>?> query, Action<Action<T1, T2, T3>?>? parallelQuery = null) {
			_query = query;
			_parallelQuery = parallelQuery;
		}

		public Query<T1, T2, T3> Each(Action<T1, T2, T3> action) {
			_each = action;
			return this;
		}

		public Query<T1, T2, T3> Parallel(bool parallel) {
			_parallel = parallel;
			return this;
		}

		public void Execute() {
			if(_parallel && _parallelQuery != null) _parallelQuery.Invoke(_each);
			else _query.Invoke(_each);
		}
	}
}