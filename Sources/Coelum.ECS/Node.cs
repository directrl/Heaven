using System.Security.Cryptography;
using Coelum.Debug;

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

		private string? _path;
		public string Path {
			get => _path ?? Name;
			private set => _path = value;
		}
		
		public NodeRoot Root { get; set; }

		private Node? _parent;
		public Node? Parent {
			get => _parent;
			private set {
				_parent = value;
				if(_parent != null) Path = _parent.Path + "." + Name;
			}
		}

		internal Node[] _defaultChildren = Array.Empty<Node>();
		public Node[] Children { init => _defaultChildren = value; }
		
		public Dictionary<Type, NodeComponent> Components { get; protected set; } = new();
		
		public void Add(params Node[] nodes) {
			Tests.Assert(Root != null,
			             "Root is unexpectedly null. If you are adding children to it before adding it to a root node," +
			             " make sure to forward-declare Root in the object initializer");
			
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