using System.Collections;
using System.Text.Json;
using Coelum.Debug;
using Coelum.ECS.Extensions;
using Serilog;

namespace Coelum.ECS.Serialization {
	
	public static partial class NodeExporter {

		public static void Export(this Node node, Stream output) {
			using var writer = new Utf8JsonWriter(output, new() {
				SkipValidation = Debugging.Enabled
			});
			
			node.Export(writer);
		}
		
		public static void Export(this Node node, Utf8JsonWriter writer) {
			if(!node.Export) {
				Log.Information($"Skipping exporting of node [{node}] due to Export flag set to false");
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
			
			// fields
			writer.WriteStartObject("fields");
			node.GetType().ExportFields(node, writer);
			writer.WriteEndObject();
			
			// properties
			writer.WriteStartObject("properties");
			node.GetType().ExportProperties(node, writer);
			writer.WriteEndObject();
			
			// components
			writer.WriteStartObject("components");
			{
				var exportedComponents = new List<INodeComponent>();
				
				foreach(var (type, component) in node.Components) {
					if(exportedComponents.Contains(component)) continue;
					
					writer.WriteStartObject(component.GetType().ToString());
					{
						writer.WriteString("backing_type", type.ToString());
						
						// fields
						writer.WriteStartObject("fields");
						component.GetType().ExportFields(component, writer);
						writer.WriteEndObject();
			
						// properties
						writer.WriteStartObject("properties");
						component.GetType().ExportProperties(component, writer);
						writer.WriteEndObject();
					}
					writer.WriteEndObject();
					
					exportedComponents.Add(component);
				}
			}
			writer.WriteEndObject();
			
			writer.WriteEndObject();
		}
		
		private static void ExportFields(this Type type, object inst, Utf8JsonWriter writer) {
			foreach(var field in type.GetFields()) {
				if(!field.IsForPublicNodeUse()) continue;

				ExportProperty(
					field.Name,
					field.GetValue(inst),
					writer
				);
			}
		}

		private static void ExportProperties(this Type type, object inst, Utf8JsonWriter writer) {
			foreach(var property in type.GetProperties()) {
				if(!property.IsForPublicNodeUse()) continue;
				
				ExportProperty(
					property.Name,
					property.GetValue(inst),
					writer
				);
			}
		}
		
		private static void ExportProperty(string pName, object? pValue, Utf8JsonWriter writer) {
			if(pValue is null) {
				writer.WriteNull(pName);
				return;
			}
			
			var pType = pValue.GetType().GetCommonTypeForNodeUse();
			
			Tests.Assert(PROPERTY_EXPORTERS.TryGetValue(pType, out var action),
				$"No exporter for {pType}");
			action.Invoke(pName, pValue, writer);
		}
	}
}