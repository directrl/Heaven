using System.Diagnostics;
using Coelum.Debug;
using Coelum.ECS.Tags;
using Coelum.LanguageExtensions.Serialization;
using Serilog;

namespace Coelum.ECS {
	
	public partial class NodeRoot {

		private ulong _lastId = 0; // TODO better way

		private Dictionary<ulong, Node> _nodes = new();
		private Dictionary<string, Node> _pathNodeMap = new();
		private Dictionary<Type, List<Node>> _componentNodeMap = new();

		private List<Action> _futureActions = new();

		public int ChildCount => _nodes.Count;
		
		public void Load(IEcsModule module) {
			module.Load(this);
		}

		[Obsolete] // TODO Obsolete
		public void AddSystem(SystemPhase phase, EcsSystem system) {
			/*if(!_systems.ContainsKey(phase)) _systems[phase] = new();
			_systems[phase].Add(system);
			
			Log.Debug($"[ECS] New system registered for phase {phase}");*/
			AddSystem(system);
		}

		public void RemoveSystem(Type system) {
			RunLater(() => {
			#region Regular systems
				foreach(var systems in _systems.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
			#endregion

			#region Child query systems
				foreach(var systems in _childQuerySystems.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
				
				foreach(var systems in _childQuerySystemsP.Values) {
					systems.RemoveAll(s => s.GetType() == system);
				}
			#endregion
			});
		}

		public void ReplaceSystem(Type what, EcsSystem with) {
			RunLater(() => {
			#region Regular systems
				foreach(var systems in _systems.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = with;
					}
				}
			#endregion

			#region Child query systems
				foreach(var systems in _childQuerySystems.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = (ChildQuerySystem) with;
					}
				}
				
				foreach(var systems in _childQuerySystemsP.Values) {
					for(int i = 0; i < systems.Count; i++) {
						if(systems[i].GetType() == what) systems[i] = (ChildQuerySystem) with;
					}
				}
			#endregion
			});
		}

		public void Process(SystemPhase phase, float delta) {
			_phaseDeltaTimes[phase] = delta;

		#region Child queries
		#region Regular
			bool queriesPresent = _childQueries.ContainsKey(phase);
			bool systemQueriesPresent = _childQuerySystems.ContainsKey(phase);

			if(systemQueriesPresent) {
				foreach(var system in _childQuerySystems[phase]) {
					system.Reset();
				}
			}
			
			if(queriesPresent || systemQueriesPresent) {
				foreach(var node in _nodes.Values) {
					if(queriesPresent) {
						foreach(var query in _childQueries[phase]) {
							query.Call(this, node);
						}
					}
				
					if(systemQueriesPresent) {
						foreach(var system in _childQuerySystems[phase]) {
							system.Invoke(this, node);
						}
					}
				}
			}
		#endregion

		#region Parallel
			queriesPresent = _childQueriesP.ContainsKey(phase);
			systemQueriesPresent = _childQuerySystemsP.ContainsKey(phase);

			if(systemQueriesPresent) {
				foreach(var system in _childQuerySystemsP[phase]) {
					system.Reset();
				}
			}

			if(queriesPresent || systemQueriesPresent) {
				Parallel.ForEach(_nodes.Values, node => {
					if(queriesPresent) {
						foreach(var query in _childQueriesP[phase]) {
							query.Call(this, node);
						}
					}
				
					if(systemQueriesPresent) {
						foreach(var system in _childQuerySystemsP[phase]) {
							system.Invoke(this, node);
						}
					}
				});
			}
		#endregion
		#endregion

		#region Regular systems
			if(_systems.TryGetValue(phase, out var systems)) {
				foreach(var system in systems) {
					system.Invoke(this, delta);
				}
			}
		#endregion

			var a = _futureActions.ToArray();
			foreach(var action in a) {
				action.Invoke();
				_futureActions.Remove(action);
			}
		}

		public void RunLater(Action action) {
			_futureActions.Add(action);
		}
		
		public TNode Add<TNode>(TNode node) where TNode : Node {
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

			node.Alive = true;
			Log.Verbose($"[ECS] Added new node {node}");
			return node;
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
					if(!_pathNodeMap.Remove(oldNodePath, out var node)) {
						continue;
					}

					_pathNodeMap[newNodePath] = node;
					node._path = newNodePath;
				}
			});
		}

		public void Remove(Node node) {
			node.Alive = false;
			
			_nodes.Remove(node.Id);
			_pathNodeMap.Remove(node.Path);

			foreach(var type in node.Components.Keys) {
				_componentNodeMap[type].Remove(node);
			}
			
			node.Id = 0;
		}

		/// <summary>
		/// Clears all nodes from this node root
		/// </summary>
		/// <param name="unexportable">Whether or not to also clear unexportable nodes</param>
		public void ClearNodes(bool unexportable = false) {
			if(unexportable) {
				_nodes.Clear();
				_pathNodeMap.Clear();

				foreach(var component in _componentNodeMap.Keys) {
					_componentNodeMap[component].Clear();
				}
				
				_lastId = 0;
			} else {
				_nodes = _nodes
				         .Where(kv => !kv.Value.Export)
				         .ToDictionary(kv => kv.Key, kv => kv.Value);
				
				_pathNodeMap = _pathNodeMap
				               .Where(kv => !kv.Value.Export)
				               .ToDictionary(kv => kv.Key, kv => kv.Value);
				
				foreach(var component in _componentNodeMap.Keys) {
					_componentNodeMap[component] = _componentNodeMap[component]
					                               .Where(node => !node.Export)
					                               .ToList();
				}

				_lastId = _nodes.Count > 0
					? _nodes.Keys.Max()
					: 0;
			}
		}

		public void ClearSystems() {
			_systems.Clear();
		}

		public Node? Get(ulong id) => _nodes.GetValueOrDefault(id);
		public Node? Get(string path) => _pathNodeMap.GetValueOrDefault(path);
		public List<Node>? Get<TComponent>() where TComponent : INodeComponent {
			return _componentNodeMap.GetValueOrDefault(typeof(TComponent));
		}

	#region Parallel add thing
		private static void AddSpTP<T>(ref Dictionary<SystemPhase, List<T>> dict,
		                        ref Dictionary<SystemPhase, List<T>> pDict,
		                        SystemPhase sp, T t, bool p) {
			if(p) {
				if(!pDict.ContainsKey(sp)) {
					pDict[sp] = new();
				}
				
				pDict[sp].Add(t);
			} else {
				if(!dict.ContainsKey(sp)) {
					dict[sp] = new();
				}
				
				dict[sp].Add(t);
			}
		}
	#endregion
	}
}