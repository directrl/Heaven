using Coelum.Debug;
using Coelum.ECS.Tags;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		/// <summary>
		/// Gets a singleton child node
		/// </summary>
		/// <typeparam name="TNode">The node type to match against</typeparam>
		/// <returns>The singleton node (if exists)</returns>
		public TNode? QuerySingleton<TNode>() where TNode : Node, new() {
			Tests.Assert(new TNode().HasComponent<Singleton>(), "Given node is not a singleton!");

			foreach(var node in _nodes.Values) {
				if(node is TNode proper) return proper;
			}

			return null;
		}
		
		/// <summary>
		/// Creates a query for all children of the current NodeRoot
		/// </summary>
		/// <param name="depth">The depth to scan children for. Set to 0 for infinite</param>
		/// <returns>The query</returns>
		public Query<Node> QueryChildren(int depth = 0) {
			return new(
				each => {
					foreach(var node in _nodes.Values) {
						if(depth <= 0) {
							each?.Invoke(node);
						} else {
							if(node.Name == node.Path) {
								each?.Invoke(node);
							}
						}
					}
				},
				each => {
					Parallel.ForEach(_nodes.Values, node => {
						if(depth <= 0) {
							each?.Invoke(node);
						} else {
							if(node.Name == node.Path) {
								each?.Invoke(node);
							}
						}
					});
				}
			);
		}
		
		/// <summary>
		/// Creates a query for all children of the current NodeRoot of given type TNode
		/// </summary>
		/// <typeparam name="TNode">Node type to match against</typeparam>
		/// <param name="depth">The depth to scan children for. Set to 0 for infinite</param>
		/// <returns>The query</returns>
		public Query<TNode> QueryChildren<TNode>(int depth = 0) where TNode : Node {
			return new(
				each => {
					foreach(var node in _nodes.Values) {
						if(depth <= 0) {
							if(node is TNode proper) each?.Invoke(proper);
						} else {
							if(node is TNode proper && node.Name == node.Path) {
								each?.Invoke(proper);
							}
						}
					}
				},
				each => {
					Parallel.ForEach(_nodes.Values, node => {
						if(depth <= 0) {
							if(node is TNode proper) each?.Invoke(proper);
						} else {
							if(node is TNode proper && node.Name == node.Path) {
								each?.Invoke(proper);
							}
						}
					});
				}
			);
		}

		/// <summary>
		/// Creates a query for all children of given node
		/// </summary>
		/// <param name="parent">The parent node to query children of</param>
		/// <param name="depth">The depth to scan children for. Set to 0 for infinite</param>
		/// <returns>The query</returns>
		public Query<Node> QueryChildren(Node parent, int depth = 0) {
			return new(
				each => {
					foreach(var (path, child) in _pathNodeMap) {
						if(child == parent) continue;
						
						if(depth <= 0) {
							if(path.StartsWith(parent.Path)) each?.Invoke(child);
						} else {
							if(string.Equals(path, parent.Path + "." + child.Name)) {
								each?.Invoke(child);
							}
						}
					}
				},
				each => {
					Parallel.ForEach(_pathNodeMap, kv => {
						var path = kv.Key;
						var child = kv.Value;
						
						if(child == parent) return;

						if(depth <= 0) {
							if(path.StartsWith(parent.Path)) each?.Invoke(child);
						} else {
							if(string.Equals(path, parent.Path + "." + child.Name)) {
								each?.Invoke(child);
							}
						}
					});
				}
			);
		}
	}
}