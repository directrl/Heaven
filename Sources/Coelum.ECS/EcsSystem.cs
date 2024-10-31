using System.Diagnostics;

namespace Coelum.ECS {
	
	public class EcsSystem {

		private readonly Stopwatch _timer = new();
		
		protected Action<NodeRoot, float> Action { get; init; }
		
		public string Name { get; }
		
		public bool Enabled { get; set; }
		public TimeSpan ExecutionTime { get; private set; }

		protected EcsSystem(string name) {
			Name = name;
		}

		public EcsSystem(string name, Action<NodeRoot, float> action) {
			Name = name;
			Action = action;
		}
		
		public void Invoke(NodeRoot root, float delta) {
			_timer.Restart();
			Action.Invoke(root, delta);
			_timer.Stop();

			ExecutionTime = _timer.Elapsed;
		}
	}
}