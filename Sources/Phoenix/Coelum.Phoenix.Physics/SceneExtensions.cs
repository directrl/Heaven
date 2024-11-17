using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using Coelum.Common.Graphics;
using Coelum.Phoenix.Physics.Callbacks;
using Coelum.Phoenix.Physics.ECS;

namespace Coelum.Phoenix.Physics {
	
	public static class SceneExtensions {

		public static (Simulation Simulation, ThreadDispatcher Dispatcher) CreatePhysicsSimulation
			<TNarrowPhaseCallbacks, TPoseIntegratorCallbacks>(this SceneBase scene, 
			                                                  INarrowPhaseCallbacks? narrowCallbacks = null, 
			                                                  IPoseIntegratorCallbacks? poseCallbacks = null, 
			                                                  SolveDescription? solveDescription = null,
			                                                  ITimestepper? timestepper = null)
			where TNarrowPhaseCallbacks : struct, INarrowPhaseCallbacks
			where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks {
			
			SimulationManager.BufferPool ??= new();
			
			narrowCallbacks ??= new DefaultNarrowPhase();
			poseCallbacks ??= new DefaultPoseIntegrator();
			solveDescription ??= new(8, 1);

			var simulation = Simulation.Create(
				SimulationManager.BufferPool,
				(TNarrowPhaseCallbacks) narrowCallbacks,
				(TPoseIntegratorCallbacks) poseCallbacks,
				solveDescription.Value,
				timestepper
			);

			var dispatcher = new ThreadDispatcher((int) Math.Ceiling(Environment.ProcessorCount / 2d));
			
			// initialize additional node property importers/exporters
			typeof(PropertyExporters).TypeInitializer?.Invoke(null, null);
			typeof(PropertyImporters).TypeInitializer?.Invoke(null, null);
			
			_ = SimulationManager.AddSimulation(scene, simulation, dispatcher);
			
			return (
				simulation,
				dispatcher
			);
		}
	}
}