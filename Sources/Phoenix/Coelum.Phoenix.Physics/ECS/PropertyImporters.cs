using BepuPhysics;
using Coelum.ECS.Serialization;
using Serilog;

namespace Coelum.Phoenix.Physics.ECS {
	
	public static class PropertyImporters {

		static PropertyImporters() {
			NodeImporter.PROPERTY_IMPORTERS[typeof(Simulation)] = json => {
				var id = json.GetValue<int>();

				if(!SimulationManager.GetById(id, out var e)) {
					Log.Error($"Could not find simulation with id {id}");
					return null;
				}

				return e.Simulation;
			};
		}
	}
}