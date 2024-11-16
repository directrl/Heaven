using System.Drawing;
using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.Core;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Nodes;
using Coelum.Resources;

namespace PhoenixPlayground.Nodes.Physics {
	
	public class Player : DynamicPhysicsBody3D {

		public Player(Simulation? simulation) : base(simulation) {
			var model = ModelLoader.Load(Heaven.AppResources[ResourceType.MODEL, "player.glb"]);
			model.Materials[0].Albedo = Color.Bisque.ToVector4();
			
			AddComponent<Renderable>(new ModelRenderable(model));
		}

		public override PhysicsBody.Shape ComputeShape() {
			var t3d = GetComponent<Transform3D>();

			var shape = new Cylinder(t3d.GlobalScale.X, t3d.GlobalScale.Y * 3f);

			return new() {
				Index = Simulation.GetStore().SetShape(this, shape),
				Inertia = shape.ComputeInertia(1.7f)
			};
		}
	}
}