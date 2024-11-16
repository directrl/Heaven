using BepuPhysics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Physics.ECS.Components;
using Serilog;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	[Obsolete] // TODO Obsolete
	public class PhysicsBodyCreateSystem : EcsSystem {

		public Simulation Simulation { get; }
		public PhysicsStore PhysicsStore { get; }
		
		public PhysicsBodyCreateSystem(Simulation simulation) : base("Physics Body Create") {
			Simulation = simulation;
			PhysicsStore = simulation.GetStore();
			
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<PhysicsBody, Transform>()
			    .Each((node, p, t) => {
				    if(p.Created) return;
				    if(t is not Transform3D t3d) return;

				    switch(p) {
					    case StaticPhysicsBody o:
						    var desc = new StaticDescription(
							    t3d.GlobalPosition,
							    t3d.QGlobalRotation,
							    o.ComputeShape().Index
							);
						    
						    Log.Verbose($"[{GetType().Name}] Adding static object to simulation");
						    
						    PhysicsStore.SetStatic(o, desc);
						    break;
				    }

				    p.Created = true;
			    })
			    .Execute();
		}
	}
}
