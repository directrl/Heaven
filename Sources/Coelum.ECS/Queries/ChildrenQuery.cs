namespace Coelum.ECS.Queries {
	
	public class ChildrenQuery<TNode> : IChildQuery
		where TNode : Node {
		
		public bool Parallel { get; set; } = false;
		public SystemPhase Phase { get; set; }
		public Action<NodeRoot, TNode> Action { get; set; }

		public ChildrenQuery(SystemPhase phase, Action<NodeRoot, TNode> action) {
			Phase = phase;
			Action = action;
		}
		
		public bool Call(NodeRoot root, Node node) {
			if(node is not TNode t) return false;
			
			Action.Invoke(root, t);
			return true;
		}

		public void Reset(NodeRoot root) { }
	}
}