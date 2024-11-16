using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.ECS;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics {
	
	public class PhysicsStore {

		public Simulation Simulation { get; }
		
		public Dictionary<StaticPhysicsBody, StaticHandle> StaticHandles = new();
		public Dictionary<PhysicsBody, BodyHandle> BodyHandles = new();

		public Dictionary<PhysicsBody, TypedIndex> Shapes = new();
		
		public PhysicsStore(Simulation simulation) {
			Simulation = simulation;
		}

		public void SetStatic(StaticPhysicsBody body, StaticDescription desc) {
			if(StaticHandles.TryGetValue(body, out var prevHandle)) {
				Simulation.Statics.Remove(prevHandle);
			}
			
			StaticHandles[body] = Simulation.Statics.Add(desc);
		}

		public bool GetStatic(StaticPhysicsBody body, out StaticHandle handle) {
			if(StaticHandles.TryGetValue(body, out handle)) {
				return true;
			}

			handle = default;
			return false;
		}

		public void SetBody<TPhysicsBody>(TPhysicsBody body, BodyDescription desc)
			where TPhysicsBody : PhysicsBody {
			
			if(BodyHandles.TryGetValue(body, out var prevHandle)) {
				Simulation.Bodies.Remove(prevHandle);
			}

			BodyHandles[body] = Simulation.Bodies.Add(desc);
		}
		
		public bool GetBody(PhysicsBody body, out BodyHandle handle) {
			if(BodyHandles.TryGetValue(body, out handle)) {
				return true;
			}

			handle = default;
			return false;
		}

		public TypedIndex SetShape<TShape>(Node node, TShape shape)
			where TShape : unmanaged, IShape {
			
			return SetShape(node.GetComponent<PhysicsBody>(), shape);
		}

		public TypedIndex SetShape<TShape>(PhysicsBody o, TShape shape)
			where TShape : unmanaged, IShape {

		#if DEBUG
			DebugShapeStore.Add(o.Owner, shape);
		#endif

			if(Shapes.TryGetValue(o, out var prevShape)) {
				Simulation.Shapes.RemoveAndDispose(prevShape, SceneExtensions._physicsBufferPool);
			}

			var ti = Simulation.Shapes.Add(shape);
			Shapes[o] = ti;
			return ti;
		}
	}
}