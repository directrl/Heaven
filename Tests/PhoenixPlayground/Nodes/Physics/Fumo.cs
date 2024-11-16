using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.Core;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Resources;

namespace PhoenixPlayground.Nodes.Physics {
	
	public class Fumo : Node3D {

		private static readonly Model _FUMO =
			ModelLoader.Load(Heaven.AppResources[ResourceType.MODEL, "okuu_fumo.glb"]);

		public Fumo() {
			AddComponent<Renderable>(new ModelRenderable(_FUMO));
		}
		
		public Fumo(Simulation simulation) : this() {
			AddComponent<PhysicsBody>(
				new DynamicPhysicsBody(
					simulation,
					() => {
						var shape = new Box(
							GetComponent<Transform3D>().GlobalScale.X,
							GetComponent<Transform3D>().GlobalScale.Y,
							GetComponent<Transform3D>().GlobalScale.Z
						);

						return (simulation.GetStore().SetShape(this, shape), shape.ComputeInertia(1));
					}
				)
			);
		}
	}
}