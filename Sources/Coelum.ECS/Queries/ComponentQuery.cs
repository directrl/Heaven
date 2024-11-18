namespace Coelum.ECS.Queries {
	
	public class ComponentQuery<TComponent> : IChildQuery
		where TComponent : INodeComponent {

		public bool Parallel { get; set; } = false;
		public SystemPhase Phase { get; set; }
		public Action<NodeRoot, TComponent> Action { get; set; }

		public ComponentQuery(SystemPhase phase,
		                      Action<NodeRoot, TComponent> action) {
			Phase = phase;
			Action = action;
		}
		
		public bool Call(NodeRoot root, Node node) {
			if(!node.TryGetComponent<TComponent>(out var t)) return false;
			
			Action.Invoke(root, t);
			return true;
		}

		public void Reset(NodeRoot root) { }
	}
	
	public class ComponentQuery<TComponent1, TComponent2> : IChildQuery
		where TComponent1 : INodeComponent
		where TComponent2 : INodeComponent {

		public bool Parallel { get; set; } = false;
		public SystemPhase Phase { get; set; }
		public Action<NodeRoot, TComponent1, TComponent2> Action { get; set; }
		
		public ComponentQuery(SystemPhase phase,
		                      Action<NodeRoot, TComponent1, TComponent2> action) {
			Phase = phase;
			Action = action;
		}
		
		public bool Call(NodeRoot root, Node node) {
			if(!node.TryGetComponent<TComponent1>(out var t1)) return false;
			if(!node.TryGetComponent<TComponent2>(out var t2)) return false;
			
			Action.Invoke(root, t1, t2);
			return true;
		}

		public void Reset(NodeRoot root) { }
	}
	
	public class ComponentQuery<TComponent1, TComponent2, TComponent3> : IChildQuery
		where TComponent1 : INodeComponent
		where TComponent2 : INodeComponent
		where TComponent3 : INodeComponent {
		
		public bool Parallel { get; set; } = false;
		public SystemPhase Phase { get; set; }
		public Action<NodeRoot, TComponent1, TComponent2, TComponent3> Action { get; set; }
		
		public ComponentQuery(SystemPhase phase,
		                      Action<NodeRoot, TComponent1, TComponent2, TComponent3> action) {
			Phase = phase;
			Action = action;
		}
		
		public bool Call(NodeRoot root, Node node) {
			if(!node.TryGetComponent<TComponent1>(out var t1)) return false;
			if(!node.TryGetComponent<TComponent2>(out var t2)) return false;
			if(!node.TryGetComponent<TComponent3>(out var t3)) return false;

			Action.Invoke(root, t1, t2, t3);
			return true;
		}

		public void Reset(NodeRoot root) { }
	}
}