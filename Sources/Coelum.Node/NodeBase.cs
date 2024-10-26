using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Coelum.LanguageExtensions;

namespace Coelum.Node {
	
	public class NodeBase {

	#region Delegates
		public delegate void RemovedEventHandler(NodeBase parent);
		public delegate void AddedEventHandler(NodeBase parent);
	#endregion

	#region Events
		public event RemovedEventHandler? Removed;
		public event AddedEventHandler? Added;
	#endregion

		private static readonly Random RANDOM = new();
		
		public NodeBase? Parent { get; internal set; }

		private Dictionary<string, NodeBase> _children;
		public ReadOnlyDictionary<string, NodeBase> Children => new(_children);

		public Dictionary<string, NodeBase> InitialChildren {
			init {
				_children = value;
				
				foreach((string key, var child) in _children) {
					child.Parent = this;
					child.Name = key;
				}
			}
		}

		private string _name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 8)
		                                            .Select(s => s[RANDOM.Next(s.Length)])
		                                            .ToArray());
		public string Name {
			set {
				if(Parent != null) {
					Parent._children.Remove(_name);
					Parent._children[value] = this;
				}
				
				_name = value;
			}
			get => _name;
		}

		public NodeBase() {
			Parent = null;
			_children = new();
		}

		public NodeBase(NodeBase? parent, Dictionary<string, NodeBase> children) {
			Parent = parent;
			_children = new(children);
		}

		public void ClearChildren() {
			_children.Clear();
		}

		public void AddChild(NodeBase child) {
			child.Parent = this;
			_children[child.Name] = child;
			child.Added?.Invoke(this);
		}
		
		public void AddChild(string newName, NodeBase child) {
			if(_children.ContainsKey(child.Name) || child.Parent != null) {
				throw new ArgumentException("child is already has a parent", nameof(child));
			}
			
			child._name = newName;
			AddChild(child);
		}

		public void RemoveChild(string name) {
			RemoveChild(Children[name]);
		}
		
		public void RemoveChild(NodeBase child) {
			_children.Remove(child.Name);
			child.Removed?.Invoke(child.Parent);
			child.Parent = null;
		}

		public TNode? FindParentByType<TNode>(NodeBase? start = null)
			where TNode : NodeBase {

			start ??= this;
			
			switch(start.Parent) {
				case null:
					return null;
				case TNode node:
					return node;
				default:
					return FindParentByType<TNode>(start.Parent);
			}
		}

		public void FindChildrenByType<TNode>(Action<TNode> forEach) where TNode : NodeBase {
			Parallel.ForEach(Children.Values, child => {
				if(child is TNode theChild) forEach.Invoke(theChild);
			});
		}
		
		public void FindChildrenByComponentParallel<TComponent>(Action<TComponent> forEach)
			where TComponent : INodeComponent {
		
			Parallel.ForEach(Children.Values, child => {
				if(child is TComponent component) {
					forEach.Invoke(component);
				}
		
				void Traverse(NodeBase node) {
					foreach(var child in node.Children.Values) {
						if(child is TComponent componentChild) {
							forEach.Invoke(componentChild);
						}

						if(child.Children.Count > 0) {
							Traverse(child);
						}
					}
				}
				
				Traverse(child);
			});
		}
		
		public void FindChildrenByComponent<TComponent>(Action<TComponent> forEach)
			where TComponent : INodeComponent {

			void Traverse(NodeBase node) {
				if(node is TComponent component) {
					forEach?.Invoke(component);
				}
				
				foreach(var child in node.Children.Values) {
					if(child is TComponent componentChild) {
						forEach?.Invoke(componentChild);

						if(child.Children.Count > 0) {
							Traverse(child);
						}
					}
				}
			}
		
			Traverse(this);
		}
	}
}