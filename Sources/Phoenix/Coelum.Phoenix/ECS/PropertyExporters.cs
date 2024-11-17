using Coelum.ECS.Serialization;

namespace Coelum.Phoenix.ECS {
	
	public static class PropertyExporters {

		static PropertyExporters() {
			NodeExporter.PROPERTY_EXPORTERS[typeof(Model)] = (name, value, writer) => {
				writer.WriteStartObject(name);
				{
					writer.WriteString("model", ((Model) value).Name);
					
					// TODO only export materials if they differ from model
					writer.WriteStartArray("materials");
					foreach(var material in ((Model) value).Materials) {
						material.Serialize(null, writer);
					}
					writer.WriteEndArray();
				}
				writer.WriteEndObject();
			};
		}
	}
}