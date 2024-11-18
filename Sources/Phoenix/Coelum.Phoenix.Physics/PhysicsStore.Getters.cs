using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Nodes;

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
		public bool GetBodyHandle(ActivePhysicsBody body, out BodyHandle handle) {
			if(BodyHandles.TryGetValue(body, out handle)) {
				return true;
			}

			handle = default;
			return false;
		}
		
		public bool GetBody(ActivePhysicsBody body, out BodyReference reference) {
			if(GetBodyHandle(body, out var handle)) {
				reference = Simulation.Bodies[handle];
				return true;
			}

			reference = default;
			return false;
		}
		
		public void RemoveBody(ActivePhysicsBody body) {
			if(!GetBodyHandle(body, out var handle)) return;
			if(!Shapes.TryGetValue(body, out var shapeIndex)) return;
			
			Simulation.Bodies.Remove(handle);
			Simulation.Shapes.RemoveAndDispose(shapeIndex, SimulationManager.BufferPool);
		}

		public bool GetBody(Node node, out BodyReference reference) {
			if(!node.TryGetComponent<ActivePhysicsBody>(out var body)) {
				reference = default;
				return false;
			}

			return GetBody(body, out reference);
		}
	#endregion

		public bool GetPhysicsNode(StaticHandle handle, out StaticBody3D? node) {
			var result = StaticHandles
				.FirstOrDefault(kv => kv.Value == handle).Key;

			if(result is null) {
				node = default;
				return false;
			}

			node = (StaticBody3D?) result.Owner;
			return true;
		}
		
		public bool GetPhysicsNode(BodyHandle handle, out ActiveBody3D? node) {
			var result = BodyHandles
			             .FirstOrDefault(kv => kv.Value == handle).Key;

			if(result is null) {
				node = default;
				return false;
			}

			node = (ActiveBody3D?) result.Owner;
			return true;
		}
	}
}