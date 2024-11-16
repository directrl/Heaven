using BepuPhysics;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Physics.ECS.Components;

namespace Coelum.Phoenix.Physics.ECS.Nodes {
	
	public abstract class PhysicsBody3D : Node3D {
		
		public Simulation? Simulation { get; }
		
		public abstract PhysicsBody.Shape ComputeShape();

		public PhysicsBody3D(Simulation? simulation) {
			Simulation = simulation;
		}
	}
}