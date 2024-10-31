using Coelum.Debug;
using Serilog;

namespace Coelum.ECS {
	
	public class NodeRoot {

		private ulong _lastId = 0; // TODO better way

		private Dictionary<ulong, Node> _nodes = new();
		
		private Dictionary<string, Node> _pathNodeMap = new();
		private Dictionary<Type, List<Node>> _componentNodeMap = new();

		private Dictionary<string, List<EcsSystem>> _systems = new();
		
		public void Load(IEcsModule module) {
			module.Load(this);
		}

		public void System(string phase, EcsSystem system) {
			if(!_systems.ContainsKey(phase)) _systems[phase] = new();
			_systems[phase].Add(system);
			
			Log.Debug($"[ECS] New system registered for phase {phase}");
		}

		public void Process(string phase, float delta) {
			if(!_systems.ContainsKey(phase)) return;
			foreach(var system in _systems[phase]) {
				system.Invoke(this, delta);
			}
		}
		
		public void Add(Node node) {
			node.Id = ++_lastId;
			node.Root = this;

			if(node._defaultChildren.Length > 0) {
				node.Add(node._defaultChildren);
				node._defaultChildren = Array.Empty<Node>();
			}
			
			_nodes[node.Id] = node;
			_pathNodeMap[node.Path] = node;

			foreach(var type in node.Components.Keys) {
				if(!_componentNodeMap.ContainsKey(type)) {
					_componentNodeMap[type] = new();
				}
				
				_componentNodeMap[type].Add(node);
			}
			
			Log.Verbose($"[ECS] Added new node {node}");
		}

		public void Remove(Node node) {
			_nodes.Remove(node.Id);
			_pathNodeMap.Remove(node.Path);

			foreach(var type in node.Components.Keys) {
				_componentNodeMap[type].Remove(node);
			}
			
			node.Id = 0;
		}

		public void ClearNodes() {
			_nodes.Clear();
			_pathNodeMap.Clear();

			foreach(var component in _componentNodeMap.Keys) {
				_componentNodeMap[component].Clear();
			}
			
			_lastId = 0;
		}

		public void ClearSystems() {
			_systems.Clear();
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

	#region Component queries
		public Query<Node, TComponent> Query<TComponent>() where TComponent : NodeComponent {
			return new(
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent))) return;
					
					foreach(var node in _componentNodeMap[typeof(TComponent)]) {
						each?.Invoke(node, (TComponent) node.Components[typeof(TComponent)]);
					}
				},
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent))) return;
					
					Parallel.ForEach(_componentNodeMap[typeof(TComponent)], node => {
						each?.Invoke(node, (TComponent) node.Components[typeof(TComponent)]);
					});
				}
			);
		}
		
		public Query<Node, TComponent1, TComponent2> Query<TComponent1, TComponent2>()
			where TComponent1 : NodeComponent
			where TComponent2 : NodeComponent {
			
			return new(
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					foreach(var node in _componentNodeMap[typeof(TComponent1)]) {
						var c1 = node.GetComponent<TComponent1>();
						var c2 = node.GetComponent<TComponent2>();
						
						if(c1 != null && c2 != null) each?.Invoke(node, c1, c2);
					}
				},
				each => {
					if(!_componentNodeMap.ContainsKey(typeof(TComponent1))
					   || !_componentNodeMap.ContainsKey(typeof(TComponent2))) return;
					
					Parallel.ForEach(_componentNodeMap[typeof(TComponent1)], node => {
						var c1 = node.GetComponent<TComponent1>();
						var c2 = node.GetComponent<TComponent2>();
						
						if(c1 != null && c2 != null) each?.Invoke(node, c1, c2);
					});
				}
			);
		}
	#endregion
	}
}