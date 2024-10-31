namespace Coelum.ECS {
	
	public class Node {
		
		private static readonly Random _RANDOM = new();
		
		public ulong Id { get; internal set; }
		
		// private string _name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 8)
		//                                             .Select(s => s[_RANDOM.Next(s.Length)])
		//                                             .ToArray());
		//
		// public string Name {
		// 	get => _name;
		// 	set {
		// 		Root.Remove(this);
		// 		_name = value;
		// 		Root.Add(this);
		// 	}
		// }
		
		public string Name { get; init; } = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 8)
																 .Select(s => s[_RANDOM.Next(s.Length)])
																 .ToArray());

		public string Path {
			get {
				if(Parent != null) {
					return Parent.Path + "." + Name;
				}

				return Name;
			}
		}
		
		public NodeRoot Root { get; set; }
		public Node? Parent { get; private set; }

		internal Node[] _defaultChildren = Array.Empty<Node>();
		public Node[] Children { init => _defaultChildren = value; }
		
		//public List<NodeComponent> Components { get; } = new();
		public Dictionary<Type, NodeComponent> Components { get; protected set; } = new();
		
		public void Add(params Node[] nodes) {
			foreach(var node in nodes) {
				node.Parent = this;
				Root.Add(node);
			}
		}

		public void Remove(params Node[] nodes) {
			foreach(var node in nodes) {
				node.Parent = null;
				Root.Remove(node);
			}
		}

		public TComponent AddComponent<TComponent>(TComponent component) where TComponent : NodeComponent {
			Components[typeof(TComponent)] = component;
			return component;
		}

		public TComponent GetComponent<TComponent>() where TComponent : NodeComponent {
			return (TComponent) Components[typeof(TComponent)];
		}
		
		public TRealComponent GetComponent<TBaseComponent, TRealComponent>()
			where TRealComponent : NodeComponent
			where TBaseComponent : NodeComponent {
			
			return (TRealComponent) Components[typeof(TBaseComponent)];
		}

		public bool HasComponent<TComponent>() where TComponent : NodeComponent {
			return Components.ContainsKey(typeof(TComponent));
		}
	}
}