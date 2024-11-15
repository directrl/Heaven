using System.Text.Json;
using Coelum.Debug;
using Serilog;

namespace Coelum.ECS.Serialization {
	
	public static class NodeExporter {

		public static void Export(this Node node, Stream output) {
			using var writer = new Utf8JsonWriter(output, new() {
				SkipValidation = Debugging.Enabled
			});
			
			node.Export(writer);
		}
		
		public static void Export(this Node node, Utf8JsonWriter writer) {
			if(!node.Export) {
				Log.Debug($"Skipping exporting of node [{node}] due to Export flag set to false");
				return;
			}
			
			Tests.Assert(node.Root is not null, "node must have a root assigned to be exported");
			
			writer.WriteStartObject();
			
			// basic data
			writer.WriteString("type", node.GetType().FullName);
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
			
			// children
			writer.WriteStartArray("children");
			{
				void WriteChild(Node child) {
					child.Export(writer);
					
					child.Root.QueryChildren(child, depth: 1)
					     .Each(WriteChild)
					     .Execute();
				}
				
				node.Root.QueryChildren(node, depth: 1)
				    .Each(WriteChild)
				    .Execute();
			}
			writer.WriteEndArray();
			
			// components
			writer.WriteStartObject("components");
			{
				foreach(var (type, component) in node.Components) {
					component.Serialize(type.ToString(), writer);
				}
			}
			writer.WriteEndObject();
			
			writer.WriteEndObject();
		}
	}
}