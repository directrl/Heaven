using BepuPhysics;
using Coelum.ECS.Serialization;

namespace Coelum.Phoenix.Physics.ECS {
	
	public static class PropertyImporters {

		static PropertyImporters() {
			NodeImporter.PROPERTY_IMPORTERS[typeof(Simulation)] = json => {
				var id = json.GetValue<uint>();
				var simulation = SimulationExtensions.GetSimulationById(id);

				if(simulation is null) {
					throw new ArgumentNullException($"Could not find simulation with id {id}");
				}

				return simulation;
			};
		}
	}
}