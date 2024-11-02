using System.Diagnostics;

namespace Coelum.ECS {
	
	public class EcsSystem {

		private readonly Stopwatch _timer = new();
		private int _stepCount = 0;
		
		protected Action<NodeRoot, float> Action { get; init; }
		
		public string Name { get; }

		public bool Enabled = true;
		public TimeSpan ExecutionTime { get; private set; }

		protected EcsSystem(string name) {
			Name = name;
		}

		public EcsSystem(string name, Action<NodeRoot, float> action) {
			Name = name;
			Action = action;
		}
		
		public void Invoke(NodeRoot root, float delta) {
			if(!Enabled && _stepCount <= 0) return;
			
			_timer.Restart();
			Action.Invoke(root, delta);
			_timer.Stop();

			if(_stepCount > 0) _stepCount--;

			ExecutionTime = _timer.Elapsed;
		}

		public void Step(int count = 1) {
			_stepCount = count;
		}
	}
}