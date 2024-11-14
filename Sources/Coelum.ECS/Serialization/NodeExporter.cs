using System.Text.Json;

namespace Coelum.ECS.Serialization {
	
	public static class NodeExporter {

		public static void Export(this Node node, Stream output) {
			using var writer = new Utf8JsonWriter(output, new() {
				SkipValidation = true
			});
			
			node.Export(writer);
		}
		
		public static void Export(this Node node, Utf8JsonWriter writer) {
			writer.WriteStartObject();
			
			// basic data
			writer.WriteString("type", node.GetType().Name);
			writer.WriteNumber("id", node.Id);
			writer.WriteBoolean("hidden", node.Hidden);
			writer.WriteString("name", node.Name);
			writer.WriteString("path", node.Path);
			
			// parent data
			if(node.Parent is not null) {
				writer.WriteStartObject("parent");
				{
					writer.WriteNumber("id", node.Parent.Id);
				}
				writer.WriteEndObject();
			}
			
			// default children
			writer.WriteStartArray("default_children");
			{
				foreach(var child in node._defaultChildren) {
					child.Export(writer);
				}
			}
			writer.WriteEndArray();
			
			// components
			writer.WriteStartArray("components");
			{
				foreach(var (type, component) in node.Components) {
					writer.WriteStartObject();
					writer.WriteString("type", type.ToString());
					writer.WriteString("real_type", component.GetType().ToString());
					component.Export(writer);
					writer.WriteEndObject();
				}
			}
			writer.WriteEndArray();
			
			writer.WriteEndObject();
		}
	}
}