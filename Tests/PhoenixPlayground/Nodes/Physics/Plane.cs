using System.Drawing;
using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Physics;
using Coelum.Phoenix.Physics.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Nodes;

namespace PhoenixPlayground.Nodes.Physics {
	
	public class Plane : StaticBody3D {

		public Plane() : this(null) { }

		public Plane(Simulation? simulation) : base(simulation) {
			AddComponent<Renderable>(new ModelRenderable(new(ColorCube.DEFAULT_MODEL)));
		}

		public override PhysicsBody.Data CreateBody() {
			var shape = new Box(
				GetComponent<Transform3D>().GlobalScale.X,
				GetComponent<Transform3D>().GlobalScale.Y,
				GetComponent<Transform3D>().GlobalScale.Z
			);

			return new() {
				Index = Simulation.GetStore().SetShape(this, shape)
			};
		}
	}
}