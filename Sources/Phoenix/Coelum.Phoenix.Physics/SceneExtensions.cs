using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using Coelum.Common.Graphics;
using Coelum.Phoenix.Physics.Callbacks;

namespace Coelum.Phoenix.Physics {
	
	public static class SceneExtensions {

		public static (Simulation, ThreadDispatcher) CreatePhysicsSimulation
			<TNarrowPhaseCallbacks, TPoseIntegratorCallbacks>(this SceneBase scene, 
			                                                  INarrowPhaseCallbacks? narrowCallbacks = null, 
			                                                  IPoseIntegratorCallbacks? poseCallbacks = null, 
			                                                  SolveDescription? solveDescription = null,
			                                                  ITimestepper? timestepper = null)
			where TNarrowPhaseCallbacks : struct, INarrowPhaseCallbacks
			where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks {
			
			PhysicsGlobals.BufferPool ??= new();
			//_threadDispatcher = new((int) Math.Ceiling(Environment.ProcessorCount / 2d));
			
			narrowCallbacks ??= new DefaultNarrowPhase();
			poseCallbacks ??= new DefaultPoseIntegrator();
			solveDescription ??= new(8, 1);
			
			return (
				Simulation.Create(
					PhysicsGlobals.BufferPool,
					(TNarrowPhaseCallbacks) narrowCallbacks,
					(TPoseIntegratorCallbacks) poseCallbacks,
					solveDescription.Value,
					timestepper
				),
				new ThreadDispatcher((int) Math.Ceiling(Environment.ProcessorCount / 2d))
			);
		}
	}
}