using BepuPhysics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics {
	
	public partial class PhysicsStore {

	#region Statics
		public void SetStatic(StaticPhysicsBody body, StaticDescription desc) {
			if(StaticHandles.TryGetValue(body, out var prevHandle)) {
				Simulation.Statics.Remove(prevHandle);
			}
			
			StaticHandles[body] = Simulation.Statics.Add(desc);
		}
	#endregion

	#region Active bodies
		public void SetBody(ActivePhysicsBody body, BodyDescription desc) {
			if(BodyHandles.TryGetValue(body, out var prevHandle)) {
				Simulation.Bodies.Remove(prevHandle);
			}

			BodyHandles[body] = Simulation.Bodies.Add(desc);
		}
	#endregion
		
	}
}