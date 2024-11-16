using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;

namespace PhoenixPlayground.Nodes.Physics {
	
	public class Plane : Node3D {
		
		public Simulation Simulation { get; }

		public Plane() {
			AddComponent<Renderable>(new ModelRenderable(new(ColorCube.DEFAULT_MODEL)));
		}

		public Plane(Simulation simulation) : this() {
			Simulation = simulation;
			
			AddComponent<PhysicsBody>(
				new StaticPhysicsBody(
					simulation,
					CreateShape
				)
			);
		}

		private TypedIndex CreateShape() {
			var shape = new Box(
				GetComponent<Transform3D>().GlobalScale.X,
				GetComponent<Transform3D>().GlobalScale.Y,
				GetComponent<Transform3D>().GlobalScale.Z
			);

			return Simulation.GetStore().SetShape(this, shape);
		}
	}
}