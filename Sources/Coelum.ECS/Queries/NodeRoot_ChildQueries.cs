namespace Coelum.ECS {
	
	public partial class NodeRoot {
		
		public Query<Node> QueryChildren() {
			return new(
				each => {
					foreach(var node in _nodes.Values) {
						each?.Invoke(node);
					}
				}
			);
		}

		public Query<Node> QueryChildren(Node parent) {
			return new(
				each => {
					foreach((var path, var child) in _pathNodeMap) {
						if(path.StartsWith(parent.Path)) each?.Invoke(child);
					}
				},
				each => {
					Parallel.ForEach(_pathNodeMap, kv => {
						var path = kv.Key;
						var child = kv.Value;

						if(path.StartsWith(parent.Path)) each?.Invoke(child);
					});
				}
			);
		}
	}
}