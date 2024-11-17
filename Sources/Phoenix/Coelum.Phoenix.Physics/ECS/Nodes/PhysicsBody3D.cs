using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class PhysicsBody3D : Node3D {

		public abstract Simulation? Simulation { get; set; }
		
		public abstract PhysicsBody.Data CreateBody();
		
		public PhysicsBody GetPhysicsComponent() => GetComponent<PhysicsBody>();
	}
}