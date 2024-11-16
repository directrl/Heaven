using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Coelum.Core;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Nodes;
using Coelum.Resources;
using Mesh = BepuPhysics.Collidables.Mesh;

namespace PhoenixPlayground.Nodes.Physics {
	
	public class Fumo : DynamicPhysicsBody3D {

		private static readonly Model _FUMO =
			ModelLoader.Load(Heaven.AppResources[ResourceType.MODEL, "okuu_fumo.glb"]);
		
		public Fumo() : this(null) { }
		
		public Fumo(Simulation? simulation) : base(simulation) {
			AddComponent<Renderable>(new ModelRenderable(_FUMO));
			GetComponent<Transform3D>().Offset = new(0, -0.25f, 0);
		}

		public override PhysicsBody.Shape ComputeShape() {
			// var shape = new Box(
			// 	GetComponent<Transform3D>().GlobalScale.X,
			// 	GetComponent<Transform3D>().GlobalScale.Y,
			// 	GetComponent<Transform3D>().GlobalScale.Z
			// );

			//var shape = GetComponent<ModelRenderable>().Model.Meshes[0].CreatePhysicsMesh();

			//var shape = new Sphere(0.5f);

			var shape = new Cylinder(0.2f, 0.5f);
						
			//return (simulation.GetStore().SetShape(this, shape), shape.ComputeInertia(1));

			return new() {
				Index = Simulation.GetStore().SetShape(this, shape),
				Inertia = shape.ComputeInertia(0.5f)
			};
		}
	}
}