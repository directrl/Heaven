using System.Text.Json;

namespace Coelum.ECS.Serialization {
	
	public static class NodeRootExporter {
		
		public static void Export(this NodeRoot root, Stream output) {
			using var writer = new Utf8JsonWriter(output, new() {
				SkipValidation = true
			});
			
			root.Export(writer);
		}

		public static void Export(this NodeRoot root, Utf8JsonWriter writer) {
			writer.WriteStartObject();
			
			// nodes
			writer.WriteStartArray("nodes");
			{
				root.QueryChildren(depth: 1)
				    .Each(node => {
					    node.Export(writer);
				    })
				    .Execute();
			}
			writer.WriteEndArray();
			
			writer.WriteEndObject();
		}
	}
}