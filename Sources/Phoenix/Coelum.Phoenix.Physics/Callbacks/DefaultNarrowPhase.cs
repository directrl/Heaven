using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;

namespace Coelum.Phoenix.Physics.Callbacks {
	
	public struct DefaultNarrowPhase : INarrowPhaseCallbacks {

		public SpringSettings ContactSpringiness { get; set; }
		public float MaximumRecoveryVelocity { get; set; }
		public float FrictionCoefficient { get; set; }

		public DefaultNarrowPhase() {
			ContactSpringiness = new(30, 1);
			MaximumRecoveryVelocity = 1.0f;
			FrictionCoefficient = 2.0f;
		}
		
		public void Initialize(Simulation simulation) { }
		
		public bool AllowContactGeneration(int workerIndex,
		                                   CollidableReference a, CollidableReference b,
		                                   ref float speculativeMargin) {

			return a.Mobility == CollidableMobility.Dynamic || b.Mobility == CollidableMobility.Dynamic;
		}
		
		public bool AllowContactGeneration(int workerIndex, CollidablePair pair,
		                                   int childIndexA, int childIndexB) {

			return true;
		}
		
		public bool ConfigureContactManifold(int workerIndex, CollidablePair pair,
		                                     int childIndexA, int childIndexB,
		                                     ref ConvexContactManifold manifold) {

			return true;
		}
		
		public bool ConfigureContactManifold<TManifold>(int workerIndex,
		                                                CollidablePair pair,
		                                                ref TManifold manifold,
		                                                out PairMaterialProperties pairMaterial)
			where TManifold : unmanaged, IContactManifold<TManifold> {

			pairMaterial = new() {
				SpringSettings = ContactSpringiness,
				MaximumRecoveryVelocity = MaximumRecoveryVelocity,
				FrictionCoefficient = FrictionCoefficient
			};
			
			return true;
		}
		
		public void Dispose() { }
	}
}