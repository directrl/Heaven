using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Coelum.Debug;

namespace Coelum.ECS {
	
	public partial class Node {
		
		private static readonly Random _RANDOM = new();
		
		public ulong Id { get; internal set; }
		public bool Hidden { get; set; }

		private string _name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 8)
		                                            .Select(s => s[_RANDOM.Next(s.Length)])
		                                            .ToArray());
		public string Name {
			get => _name;
			set {
				if(Root is not null) {
					string newPath = _parent is null ? value : _parent.Path + "." + value;
					Path = newPath;
				}
				
				_name = value;
			}
		}

		internal string? _path;
		public string Path {
			get => _path ?? Name;
			internal set {
				if(Root is not null) {
					Root.Remap(this, value);
				}
				
				_path = value;
			}
		}

		public string PathDirectory {
			get {
				var p = Path.Replace(Name, "");
				if(p.Length > 0) return p[..^1];
				else return p;
			}
		}
		
		public NodeRoot? Root { get; set; }

		private Node? _parent;
		public Node? Parent {
			get => _parent;
			set {
				Path = value is null ? Name : value.Path + "." + Name;
				_parent = value;
			}
		}

		internal Node[] _defaultChildren = Array.Empty<Node>();
		public Node[] Children { init => _defaultChildren = value; }
		
		public Dictionary<Type, INodeComponent> Components { get; protected set; } = new();
		
		public void Add(params Node[] nodes) {
			if(Root is null) {
				throw new ArgumentNullException(nameof(Root),
				                                "Root is unexpectedly null." +
				                                " If you are adding children to it before adding it to a root node," +
				                                " make sure to forward-declare Root in the object initializer");
			}
			
			foreach(var node in nodes) {
				node.Parent = this;
				Root.Add(node);
			}
		}

		public void Remove(params Node[] nodes) {
			Tests.Assert(Root != null);
			
			foreach(var node in nodes) {
				node.Parent = null;
				Root.Remove(node);
			}
		}
	}
}