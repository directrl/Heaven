using Coelum.ECS.Serialization;

namespace Coelum.Phoenix.ECS {
	
	public static class PropertyExporters {

		static PropertyExporters() {
			NodeExporter.PROPERTY_EXPORTERS[typeof(Model)] = (name, value, writer) => {
				writer.WriteStartObject(name);
				{
					writer.WriteString("model", ((Model) value).Name);
				}
				writer.WriteEndObject();
			};
		}
	}
}