using System.Collections;

namespace Coelum.Node {
	
	public class NodeList<T> : List<T> where T : NodeBase {

		private readonly NodeBase _parent;
		
		public NodeList(NodeBase parent) {
			_parent = parent;
		}
		
		public NodeList(NodeBase parent, int capacity) : base(capacity) {
			_parent = parent;
		}
		
		public NodeList(NodeBase parent, IEnumerable<T> collection) : base(collection) {
			_parent = parent;

			foreach(var item in collection) {
				item.Parent = _parent;
			}
		}
		
		public new void Add(T item) {
			item.Parent = _parent;
			base.Add(item);
		}

		public new void AddRange(IEnumerable<T> collection) {
			foreach(var item in collection) {
				item.Parent = _parent;
			}
			
			base.AddRange(collection);
		}

		public new bool Remove(T item) {
			item.Parent = null;
			return base.Remove(item);
		}
	}
}