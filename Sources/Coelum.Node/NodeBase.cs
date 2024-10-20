using System.Collections.Concurrent;
using Coelum.LanguageExtensions;

namespace Coelum.Node {
	
	public class NodeBase {
		
		public NodeBase? Parent { get; internal set; }

		private NodeList<NodeBase> _children;
		public NodeList<NodeBase> Children {
			get => _children;
			set {
				_children = value;

				foreach(var child in _children) {
					child.Parent = this;
				}
			}
		}

		public string Name { get; set; } = "";

		public NodeBase() {
			Parent = null;
			Children = new(this);
		}

		public NodeBase(NodeBase? parent, IEnumerable<NodeBase> children) {
			Parent = parent;
			Children = new(this, children);
		}

		public TNode? FindParent<TNode>(NodeBase? start = null)
			where TNode : NodeBase {

			if(start == null) start = this;
			if(start.Parent == null) return null;
			
			if(start.Parent is TNode node) {
				return node;
			}
			
			return FindParent<TNode>(start.Parent);
		}

		public NodeList<TNode> FindChildren<TNode>() where TNode : NodeBase {
			var nodes = new NodeList<TNode>(this);

			Parallel.ForEach(Children, child => {
				if(child is TNode theChild) nodes.Add(theChild);
			});
			
			return nodes;
		}
		
		public ConcurrentBag<TComponent> FindChildrenByComponent<TComponent>(NodeBase? node = null)
			where TComponent : INodeComponent {
    
			node ??= this;
			var components = new ConcurrentBag<TComponent>();
		
			Parallel.ForEach(node.Children, child => {
				if(child is TComponent a) {
					components.Add(a);
				}
		
				void Traverse(NodeBase n) {
					foreach(var c in n.Children) {
						if(c is TComponent nodeComponent) {
							components.Add(nodeComponent);
						}
						
						if(child.Children.Count > 0) Traverse(c);
					}
				}
				
				Traverse(child);
			});
		
			return components;
		}
	}
}