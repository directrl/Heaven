namespace Coelum.Node {
	
	public class Node {
		
		private static readonly Random _RANDOM = new();
		
		public ulong Id { get; internal set; }
		
		private string _name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 8)
		                                            .Select(s => s[_RANDOM.Next(s.Length)])
		                                            .ToArray());

		public string Name {
			get => _name;
			set {
				Root?.Remove(this);
				_name = value;
				Root?.Add(this);
			}
		}

		public string Path {
			get {
				if(Parent != null) {
					return Parent.Path + "." + Name;
				}

				return Name;
			}
		}
		
		public NodeRoot? Root { get; internal set; }
		public Node? Parent { get; private set; }
		
		//public List<NodeComponent> Components { get; } = new();
		public virtual Dictionary<Type, NodeComponent> Components { get; protected set; }

		public void Add(Node node) {
			node.Parent = this;
			Root?.Add(node);
		}

		public void Remove(Node node) {
			node.Parent = null;
			Root?.Remove(node);
		}
	}
}