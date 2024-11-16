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
	#region Internals
		internal void SetBody(PhysicsBody body, BodyDescription desc) {
			if(BodyHandles.TryGetValue(body, out var prevHandle)) {
				Simulation.Bodies.Remove(prevHandle);
			}

			BodyHandles[body] = Simulation.Bodies.Add(desc);
		}
	#endregion

		public void SetBody(DynamicPhysicsBody body, BodyDescription desc)
			=> SetBody((PhysicsBody) body, desc);
		
		public void SetBody(KinematicPhysicsBody body, BodyDescription desc)
			=> SetBody((PhysicsBody) body, desc);
	#endregion

	}
}