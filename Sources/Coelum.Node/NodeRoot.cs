using Coelum.Debug;

namespace Coelum.Node {
	
	public class NodeRoot {

		private ulong _lastId = 0; // TODO better way

		private Dictionary<ulong, Node> _nodes = new();
		
		private Dictionary<string, Node> _pathNodeMap = new();
		private Dictionary<Type, List<Node>> _componentNodeMap = new();

		public void RegisterComponent<TComponent>() where TComponent : NodeComponent {
			_componentNodeMap[typeof(TComponent)] = new();
		}
		
		public void Add(Node node) {
			node.Id = ++_lastId;
			node.Root = this;
			
			_nodes[node.Id] = node;
			_pathNodeMap[node.Path] = node;

			foreach(var component in node.Components) {
				Tests.Assert(_componentNodeMap.ContainsKey(component.GetType()));
				_componentNodeMap[component.GetType()].Add(node);
			}
		}

		public void Remove(Node node) {
			_nodes.Remove(node.Id);
			_pathNodeMap.Remove(node.Path);

			foreach(var component in node.Components) {
				_componentNodeMap[component.GetType()].Remove(node);
			}
			
			node.Id = 0;
			node.Root = null;
		}

		public void Clear() {
			_nodes.Clear();
			_pathNodeMap.Clear();

			foreach(var component in _componentNodeMap.Keys) {
				_componentNodeMap[component].Clear();
			}
			
			_lastId = 0;
		}

		public Node? Get(ulong id) => _nodes.GetValueOrDefault(id);
		public Node? Get(string path) => _pathNodeMap.GetValueOrDefault(path);
		public List<Node>? Get<TComponent>() where TComponent : NodeComponent {
			return _componentNodeMap.GetValueOrDefault(typeof(TComponent));
		}

		// public List<NodeV2> Children(NodeV2 parent) {
		// 	var children = new List<NodeV2>();
		// 	
		// 	foreach((var path, var child) in _pathNodeMap) {
		// 		if(path.StartsWith(parent.Path)) children.Add(child);
		// 	}
		//
		// 	return children;
		// }

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

		public Query<TComponent> QueryComponent<TComponent>() where TComponent : NodeComponent {
			return new(
				each => {
					foreach(var node in _componentNodeMap[typeof(TComponent)]) {
						each?.Invoke((TComponent) node.Components[typeof(TComponent)]);
					}
				},
				each => {
					Parallel.ForEach(_componentNodeMap[typeof(TComponent)], node => {
						each?.Invoke((TComponent) node.Components[typeof(TComponent)]);
					});
				}
			);
		}
	}
}