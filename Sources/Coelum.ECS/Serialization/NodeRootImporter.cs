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
			
			var nodes = json["nodetree"].AsArray();

			bool CreateAndImport(JsonNode nodeJson) {
				var nodeTypeName = nodeJson["type"].GetValue<string>();
				var nodeType = TypeExtensions.FindType(nodeTypeName);
				
				if(nodeType is null) {
					Log.Warning($"[NodeRootImporter] Could not find node type [{nodeTypeName}]");
					return false;
				}

				var ctor = nodeType.GetConstructor(Type.EmptyTypes);
				
				if(ctor is null) {
					Log.Warning($"[NodeRootImporter] Could not find a default constructor for node type [{nodeTypeName}]");
					return false;
				}

				var node = (Node) ctor.Invoke(null);
				node.Root = root;
				node.Import(nodeJson);

				return true;
			}
			
			foreach(var nodeJson in nodes) {
				if(!CreateAndImport(nodeJson)) continue;
				
				// node children
				var children = nodeJson["children"].AsArray();

				foreach(var childJson in children) {
					_ = CreateAndImport(childJson);
				}
			}
		}
	}
}