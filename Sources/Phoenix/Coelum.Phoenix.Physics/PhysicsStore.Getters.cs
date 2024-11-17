using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics {
	
	public partial class PhysicsStore {

	#region Statics
		public bool GetStaticHandle(StaticPhysicsBody body, out StaticHandle handle) {
			if(StaticHandles.TryGetValue(body, out handle)) {
				return true;
			}

			handle = default;
			return false;
		}
		
		public bool GetStatic(StaticPhysicsBody body, out StaticReference reference) {
			if(GetStaticHandle(body, out var handle)) {
				reference = Simulation.Statics[handle];
				return true;
			}

			reference = default;
			return false;
		}
		
		public bool GetStatic(Node node, out StaticReference reference) {
			if(!node.TryGetComponent<StaticPhysicsBody>(out var body)) {
				reference = default;
				return false;
			}

			return GetStatic(body, out reference);
		}

		public void RemoveStatic(StaticPhysicsBody body) {
			if(!GetStaticHandle(body, out var handle)) return;
			if(!Shapes.TryGetValue(body, out var shapeIndex)) return;
			
			Simulation.Statics.Remove(handle);
			Simulation.Shapes.RemoveAndDispose(shapeIndex, SimulationManager.BufferPool);
		}
	#endregion
		
	#region Active bodies
	#region Internals
		internal bool GetBodyHandle(PhysicsBody body, out BodyHandle handle) {
			if(BodyHandles.TryGetValue(body, out handle)) {
				return true;
			}

			handle = default;
			return false;
		}
		
		internal bool GetBody(PhysicsBody body, out BodyReference reference) {
			if(GetBodyHandle(body, out var handle)) {
				reference = Simulation.Bodies[handle];
				return true;
			}

			reference = default;
			return false;
		}
		
		internal void RemoveBody(PhysicsBody body) {
			if(!GetBodyHandle(body, out var handle)) return;
			if(!Shapes.TryGetValue(body, out var shapeIndex)) return;
			
			Simulation.Bodies.Remove(handle);
			Simulation.Shapes.RemoveAndDispose(shapeIndex, SimulationManager.BufferPool);
		}
	#endregion

		public bool GetBodyHandle(DynamicPhysicsBody body, out BodyHandle handle)
			=> GetBodyHandle((PhysicsBody) body, out handle);
		
		public bool GetBody(DynamicPhysicsBody body, out BodyReference reference)
			=> GetBody((PhysicsBody) body, out reference);
		
		public bool GetBodyHandle(KinematicPhysicsBody body, out BodyHandle handle)
			=> GetBodyHandle((PhysicsBody) body, out handle);
		
		public bool GetBody(KinematicPhysicsBody body, out BodyReference reference)
			=> GetBody((PhysicsBody) body, out reference);

		public bool GetBody(Node node, out BodyReference reference) {
			if(!node.HasComponent<DynamicPhysicsBody>()
			   && !node.HasComponent<KinematicPhysicsBody>()) {
				reference = default;
				return false;
			}

			var body = node.GetComponent<PhysicsBody>();
			return GetBody(body, out reference);
		}

		public void RemoveBody(DynamicPhysicsBody body)
			=> RemoveBody((PhysicsBody) body);
		
		public void RemoveBody(KinematicPhysicsBody body)
			=> RemoveBody((PhysicsBody) body);
	#endregion


	}
}