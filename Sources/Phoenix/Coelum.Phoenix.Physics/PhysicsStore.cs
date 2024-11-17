using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.Debug;
using Coelum.ECS;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics {
	
	public partial class PhysicsStore {

		public Simulation Simulation { get; }
		
		public Dictionary<StaticPhysicsBody, StaticHandle> StaticHandles = new();
		public Dictionary<PhysicsBody, BodyHandle> BodyHandles = new();

		public Dictionary<PhysicsBody, TypedIndex> Shapes = new();
		
		public PhysicsStore(Simulation simulation) {
			Simulation = simulation;
		}

		public TypedIndex SetShape<TShape>(Node node, TShape shape)
			where TShape : unmanaged, IShape {
			
			return SetShape(node.GetComponent<PhysicsBody>(), shape);
		}

		public TypedIndex SetShape<TShape>(PhysicsBody body, TShape shape)
			where TShape : unmanaged, IShape {

		#if DEBUG
			DebugShapeRenderer.Add(body.Owner, shape);
		#endif

			if(Shapes.TryGetValue(body, out var prevShape)) {
				Simulation.Shapes.RemoveAndDispose(prevShape, Simulation.BufferPool);
			}

			var ti = Simulation.Shapes.Add(shape);
			Shapes[body] = ti;
			return ti;
		}
	}
}