using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.Debug;
using Serilog;
using TypeExtensions = Coelum.LanguageExtensions.TypeExtensions;

namespace Coelum.ECS.Serialization {
	
	public static class NodeRootImporter {
		
		public static void Import(this NodeRoot root, Stream input) {
			var json = JsonNode.Parse(input, new() {
				PropertyNameCaseInsensitive = true
			});

			if(json is null) {
				throw new JsonException("Could not parse a JsonNode from stream");
			}
			
			root.Import(json);
		}

		public static void Import(this NodeRoot root, JsonNode json) {
			root.ClearNodes();
			
			var nodes = json["nodes"].AsArray();

			foreach(var nodeJson in nodes) {
				var nodeTypeName = nodeJson["type"].GetValue<string>();
				var nodeType = TypeExtensions.FindType(nodeTypeName);
				
				if(nodeType is null) {
					Log.Warning($"[NodeRootImporter] Could not find node type [{nodeTypeName}]");
					continue;
				}

				var ctor = nodeType.GetConstructor(Type.EmptyTypes);
				
				if(ctor is null) {
					Log.Warning($"[NodeRootImporter] Could not find a default constructor for node type [{nodeTypeName}]");
					continue;
				}

				var node = (Node) ctor.Invoke(null);
				node.Root = root;
				node.Import(nodeJson);
				
				root.Add(node);
			}
		}
	}
}