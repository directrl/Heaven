using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.Debug;
using Coelum.LanguageExtensions;
using Serilog;

namespace Coelum.ECS.Serialization {
	
	public static class NodeImporter {

		public static void Import(this Node node, Stream input) {
			var json = JsonNode.Parse(input, new() {
				PropertyNameCaseInsensitive = true
			});

			if(json is null) {
				throw new JsonException("Could not parse a JsonNode from stream");
			}
			
			node.Import(json);
		}

		public static void Import(this Node node, JsonNode json) {
			Tests.Assert(node.Root is not null, "node must already have root assigned");
			
			// basic data
			node.Id = json["id"].GetValue<ulong>();
			node.Hidden = json["hidden"].GetValue<bool>();
			node._name = json["name"].GetValue<string>();
			node._path = json["path"].GetValue<string>();

			// parent
			if(json["parent"] is not null) {
				// we run it at a later time to make sure all nodes are imported
				node.Root.RunLater(() => {
					var pId = json["parent"]["id"].GetValue<ulong>();
					var p = node.Root.QueryChild(pId);

					if(p is null) {
						Log.Warning($"[NodeImporter] Could not find parent with ID {pId} for {node}");
					} else {
						node.Parent = p;
					}
				});
			}
			
			// components
			var components = json["components"].AsObject();

			foreach(var componentJson in components) {
				string type = componentJson.Key;
				string backingType = componentJson.Value["backing_type"].GetValue<string>();

				var backingComponentType = TypeExtensions.FindType(backingType);
				var componentType = TypeExtensions.FindType(type);
				
				if(backingComponentType is null || componentType is null) {
					Log.Warning($"[NodeImporter] Could not find component type [{type}]");
					continue;
				}

				if(node.Components.TryGetValue(backingComponentType, out var component)) {
					component = component.Deserialize(componentJson.Value);
					component.Owner = node;
				} else {
					var ctor = componentType.GetConstructor(Type.EmptyTypes);
				
					if(ctor is null) {
						Log.Warning($"[NodeImporter] Could not find a default constructor for component type [{type}]");
						continue;
					}

					component = (INodeComponent) ctor.Invoke(null);
					component = component.Deserialize(componentJson.Value);
					component.Owner = node;
				
					node.Components[backingComponentType] = component;
				}
			}
			
			if(node.Root.QueryChild(node.Id) is null) {
				node.Root.Add(node);
			}
		}
	}
}