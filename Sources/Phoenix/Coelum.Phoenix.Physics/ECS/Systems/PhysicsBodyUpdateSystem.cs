using System.Numerics;
using BepuPhysics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Physics.ECS.Components;
using Hexa.NET.ImGuizmo;
using Serilog;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class PhysicsBodyUpdateSystem : EcsSystem {

		public Simulation Simulation { get; }
		public PhysicsStore PhysicsStore { get; }
		
		public PhysicsBodyUpdateSystem(Simulation simulation) : base("Physics Body Update") {
			Simulation = simulation;
			PhysicsStore = simulation.GetStore();
			
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<PhysicsBody, Transform>()
			    .Each((node, p, t) => {
				    if(!p.DoUpdates) return;
				    if(t is not Transform3D t3d) return;

				    if(p.Simulation is null) return;

				    // TODO move to extension method or Transform3D itself

				    switch(p) {
					    case StaticPhysicsBody body:
						    if(body.Dirty || !PhysicsStore.GetStaticHandle(body, out var staticHandle)) {
							    var desc = new StaticDescription(
								    t3d.GlobalPosition,
								    t3d.QGlobalRotation,
								    body.ComputeShape().Index
							    );
							    
							    PhysicsStore.SetStatic(body, desc);
							    body.Dirty = false;
						    } else {
							    Simulation.Statics.GetDescription(staticHandle, out var desc);
							    t3d.UpdateFromComponents(desc.Pose.Position, desc.Pose.Orientation, t3d.Scale);
						    }
						    
						    break;
					    case DynamicPhysicsBody body:
						    if(body.Dirty || !PhysicsStore.GetBodyHandle(body, out var dynamicHandle)) {
							    var shape = body.ComputeShape();

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
					    case KinematicPhysicsBody body:
						    if(body.Dirty || !PhysicsStore.GetBodyHandle(body, out var kinematicHandle)) {
							    var desc = BodyDescription.CreateKinematic(
								    (t3d.GlobalPosition, t3d.QGlobalRotation),
								    body.ComputeShape().Index,
								    0.01f
							    );
							    
							    PhysicsStore.SetBody(body, desc);
							    body.Dirty = false;
						    } else {
							    Simulation.Bodies.GetDescription(kinematicHandle, out var desc);
							    t3d.UpdateFromComponents(desc.Pose.Position, desc.Pose.Orientation, t3d.Scale);
						    }
						    
						    break;
				    }
			    })
			    .Execute();
		}
	}
}
