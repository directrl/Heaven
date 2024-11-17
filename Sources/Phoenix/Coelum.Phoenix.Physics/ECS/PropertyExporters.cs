using BepuPhysics;
using Coelum.ECS.Serialization;

namespace Coelum.Phoenix.Physics.ECS {
	
	public static class PropertyExporters {

		static PropertyExporters() {
			NodeExporter.PROPERTY_EXPORTERS[typeof(Simulation)] = (name, value, writer) => {
				writer.WriteNumber(name, SimulationExtensions.GetSimulationId((Simulation) value).Value);
			};
		}
	}
}