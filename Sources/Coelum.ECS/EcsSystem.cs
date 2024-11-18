using System.Diagnostics;
using Coelum.ECS.Queries;

namespace Coelum.ECS {
	
	public enum SystemPhase {
			
		RENDER,
		RENDER_PRE,
		RENDER_POST,
		UPDATE,
		UPDATE_PRE,
		UPDATE_POST,
		FIXED_UPDATE,
		FIXED_UPDATE_PRE,
		FIXED_UPDATE_POST,
	}
	
	public class EcsSystem {

		protected Stopwatch Timer { get; } = new();
		protected int StepCount { get; set; }
		
		protected virtual Action<NodeRoot, float> Action { get; init; }
		
		public virtual string Name { get; }
		public virtual SystemPhase Phase { get; }

		public bool Enabled = true;
		public TimeSpan ExecutionTime { get; protected set; }

		protected EcsSystem() { }
		
		[Obsolete]
		protected EcsSystem(string name, SystemPhase phase) {
			Name = name;
			Phase = phase;
		}

		public EcsSystem(string name, SystemPhase phase, Action<NodeRoot, float> action) {
			Name = name;
			Phase = phase;
			Action = action;
		}
		
		public virtual void Invoke(NodeRoot root, float delta) {
			if(!Enabled && StepCount <= 0) return;
			
			Timer.Restart();
			Action.Invoke(root, delta);
			Timer.Stop();

			if(StepCount > 0) StepCount--;

			ExecutionTime = Timer.Elapsed;
		}

		public void Step(int count = 1) {
			StepCount = count;
		}
	}

	public class ChildQuerySystem : EcsSystem {

		protected virtual IChildQuery Query { get; init; }

		protected ChildQuerySystem() { }
		
		public ChildQuerySystem(string name, SystemPhase phase, IChildQuery query) : base(name, phase) {
			Query = query;
		}

		public override void Invoke(NodeRoot root, float delta) {
			throw new NotSupportedException();
		}

		public void Invoke(NodeRoot root, Node child) {
			if(!Enabled && StepCount <= 0) return;
			
			Timer.Restart();
			Query.Call(root, child);
			Timer.Stop();

			if(StepCount > 0) StepCount--;

			ExecutionTime = Timer.Elapsed;
		}
	}
}