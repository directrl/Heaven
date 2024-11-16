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

		public TypedIndex SetShape<TShape>(PhysicsBody o, TShape shape)
			where TShape : unmanaged, IShape {

		#if DEBUG
			DebugShapeRenderer.Add(o.Owner, shape);
		#endif

			if(Shapes.TryGetValue(o, out var prevShape)) {
				Simulation.Shapes.RemoveAndDispose(prevShape, PhysicsGlobals.BufferPool);
			}

			var ti = Simulation.Shapes.Add(shape);
			Shapes[o] = ti;
			return ti;
		}
	}
}