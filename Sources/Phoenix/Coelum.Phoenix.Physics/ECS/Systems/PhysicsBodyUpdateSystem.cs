using System.Numerics;
using BepuPhysics;
using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.Physics.ECS.Components;
using Hexa.NET.ImGuizmo;
using Serilog;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class PhysicsBodyUpdateSystem : ChildQuerySystem {

		public override string Name => "Physics Body Update";
		public override SystemPhase Phase => SystemPhase.FIXED_UPDATE;

		public Simulation Simulation { get; }
		public PhysicsStore PhysicsStore { get; }
		
		public PhysicsBodyUpdateSystem(Simulation simulation) {
			Simulation = simulation;
			PhysicsStore = simulation.GetStore()
				?? throw new Exception("Simulation must have a PhysicsStore assigned");

			Query = new ComponentQuery<PhysicsBody, Transform>(Phase, QueryAction) {
				Parallel = true
			};
		}

		private void QueryAction(NodeRoot root, PhysicsBody p, Transform t) {
			if(!p.DoUpdates) return;
			if(t is not Transform3D t3d) return;

			if(p.Simulation is null) return;

			switch(p) {
				case StaticPhysicsBody body:
					if(body.Dirty || !PhysicsStore.GetStaticHandle(body, out var staticHandle)) {
						var desc = new StaticDescription(
							t3d.GlobalPosition,
							t3d.QGlobalRotation,
							body.CreateData().Index
						);
							    
						PhysicsStore.SetStatic(body, desc);
						body.Dirty = false;
					} else {
						Simulation.Statics.GetDescription(staticHandle, out var desc);
						t3d.UpdateFromComponents(desc.Pose.Position, desc.Pose.Orientation, t3d.Scale);
					}
					
					break;
				case ActivePhysicsBody body:
					if(body.Dirty || !PhysicsStore.GetBodyHandle(body, out var dynamicHandle)) {
						var shape = body.CreateData();

						var desc = BodyDescription.CreateDynamic(
							(t3d.GlobalPosition, t3d.QGlobalRotation),
							shape.Inertia,
							shape.Index,
							0.01f
						);
							    
						PhysicsStore.SetBody(body, desc);
						body.Dirty = false;
					} else {
						Simulation.Bodies.GetDescription(dynamicHandle, out var desc);
						t3d.UpdateFromComponents(desc.Pose.Position, desc.Pose.Orientation, t3d.Scale);
					}
					
					break;
			}
		}
	}
}
