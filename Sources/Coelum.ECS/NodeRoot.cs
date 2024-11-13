using Coelum.Debug;
using Coelum.ECS.Tags;
using Serilog;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private ulong _lastId = 0; // TODO better way

		private Dictionary<ulong, Node> _nodes = new();
		private Dictionary<string, Node> _pathNodeMap = new();
		private Dictionary<Type, List<Node>> _componentNodeMap = new();

		private Dictionary<string, List<EcsSystem>> _systems = new();

		private List<Action> _futureActions = new();

		public int ChildCount => _nodes.Count;
		
		public void Load(IEcsModule module) {
			module.Load(this);
		}

		public void AddSystem(string phase, EcsSystem system) {
			if(!_systems.ContainsKey(phase)) _systems[phase] = new();
			_systems[phase].Add(system);
			
			Log.Debug($"[ECS] New system registered for phase {phase}");
		}

		public void Process(string phase, float delta) {
			if(!_systems.ContainsKey(phase)) return;
			foreach(var system in _systems[phase]) {
				system.Invoke(this, delta);
			}
			
			foreach(var action in _futureActions) {
				action.Invoke();
			}
			
			_futureActions.Clear();
		}

		public void RunLater(Action action) {
			_futureActions.Add(action);
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

				if(type == typeof(Singleton) && _componentNodeMap[type].Contains(node)) {
					throw new InvalidOperationException("Cannot add more than one instance of a singleton node type");
				}
				
				_componentNodeMap[type].Add(node);
			}
			
			Log.Verbose($"[ECS] Added new node {node}");
		}

		internal void Remap(Node node, string newPath) {
			var pathsToReplace = new Dictionary<string, string>();

			void UpdatePath(Node node, string newPath) {
				pathsToReplace[node.Path] = newPath;
				
				QueryChildren(node, depth: 1)
					.Each(child => {
						var newChildPath = newPath + "." + child.Name;
						UpdatePath(child, newChildPath);
					})
					.Execute();
			}
			
			UpdatePath(node, newPath);

			RunLater(() => {
				foreach(var (oldNodePath, newNodePath) in pathsToReplace) {
					var node = _pathNodeMap[oldNodePath];
					_pathNodeMap.Remove(oldNodePath);
					_pathNodeMap[newNodePath] = node;

					node._path = newNodePath;
				}
			});
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
		public List<Node>? Get<TComponent>() where TComponent : INodeComponent {
			return _componentNodeMap.GetValueOrDefault(typeof(TComponent));
		}
	}
}