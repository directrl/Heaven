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
		
		public virtual string Name { get; protected init; }
		public virtual SystemPhase Phase { get; init; }

		public bool Enabled = true;
		public TimeSpan ExecutionTime { get; protected set; }

		protected EcsSystem() { }

		public EcsSystem(string name, SystemPhase phase, Action<NodeRoot, float> action) {
			Name = name;
			Phase = phase;
			Action = action;
		}
		
		public virtual void Invoke(NodeRoot root, float delta) {
			if(!Enabled && StepCount <= 0) return;

		#if DEBUG
			Timer.Restart();
		#endif
			Action.Invoke(root, delta);
		#if DEBUG
			Timer.Stop();
		#endif

			if(StepCount > 0) StepCount--;

		#if DEBUG
			ExecutionTime = Timer.Elapsed;
		#endif
		}
		
		public virtual void Reset() { }

		public void Step(int count = 1) {
			StepCount = count;
		}
	}

	public abstract class QuerySystem : EcsSystem {
		
		protected Stopwatch SingleTimer { get; } = new();
		
		public TimeSpan SingleExecutionTime { get; protected set; }
		public int QueryMatches { get; protected set; }

		public override void Invoke(NodeRoot root, float delta) {
			throw new NotSupportedException();
		}
		
		public override void Reset() {
		#if DEBUG
			Timer.Reset();
			ExecutionTime = TimeSpan.Zero;
			QueryMatches = 0;
		#endif
			
			if(StepCount > 0) StepCount--;
		}
	}

	public class ChildQuerySystem : QuerySystem {
		
		public virtual IChildQuery Query { get; protected init; }

		protected ChildQuerySystem() { }
		
		public ChildQuerySystem(string name, SystemPhase phase, IChildQuery query) {
			Name = name;
			Phase = phase;
			Query = query;
		}

		public bool Invoke(NodeRoot root, Node child) {
			if(!Enabled && StepCount <= 0) return false;
			bool ret = false;

		#if DEBUG
			Timer.Start();
			SingleTimer.Restart();
		#endif

			ret = Query.Call(root, child);
		#if DEBUG
			if(ret) QueryMatches++;
		#endif
			
			if(StepCount > 0) StepCount--;

		#if DEBUG
			Timer.Stop();
			SingleTimer.Stop();
			
			ExecutionTime = Timer.Elapsed;
			SingleExecutionTime = SingleTimer.Elapsed;
		#endif

			return ret;
		}
	}
}