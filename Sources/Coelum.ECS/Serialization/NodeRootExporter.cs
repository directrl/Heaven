using System.Text.Json;
using Coelum.Debug;

namespace Coelum.ECS.Serialization {
	
	public static class NodeRootExporter {
		
		public static void Export(this NodeRoot root, Stream output) {
			using var writer = new Utf8JsonWriter(output, new() {
				SkipValidation = Debugging.Enabled
			});
			
			root.Export(writer);
		}

		public static void Export(this NodeRoot root, Utf8JsonWriter writer) {
			writer.WriteStartObject();
			
			// nodes
			/*writer.WriteStartArray("nodes");
			{
				root.QueryChildren()
				    .Each(node => {
					    node.Export(writer);
				    })
				    .Execute();
			}
			writer.WriteEndArray();*/
			
			writer.WriteStartArray("nodetree");
			{
				root.QueryChildren(depth: 1)
				    .Each(node => node.Export(writer))
				    .Execute();
			}
			writer.WriteEndArray();
			
			writer.WriteEndObject();
		}
	}
}