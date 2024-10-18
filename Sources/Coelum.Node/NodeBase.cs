namespace Coelum.Node {
	
	public class NodeBase {
		
		public NodeBase? Parent { get; protected set; }
		public List<NodeBase> Children { get; protected set; }

		public NodeBase() {
			Parent = null;
			Children = new();
		}

		public NodeBase(NodeBase? parent, IEnumerable<NodeBase> children) {
			Parent = parent;
			Children = new(children);
		}
	}
}