using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.Debug;
using Coelum.ECS.Extensions;
using Coelum.LanguageExtensions;
using Serilog;

namespace Coelum.ECS.Serialization {
	
	public static partial class NodeImporter {

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
			
			ImportFields(node.GetType(), node, json);
			ImportProperties(node.GetType(), node, json);
			
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
					ImportFields(component.GetType(), component, componentJson.Value);
					ImportProperties(component.GetType(), component, componentJson.Value);
					
					component.Owner = node;
				} else {
					var ctor = componentType.GetConstructor(Type.EmptyTypes);
				
					if(ctor is null) {
						Log.Warning($"[NodeImporter] Could not find a default constructor for component type [{type}]");
						continue;
					}

					component = (INodeComponent) ctor.Invoke(null);
					
					ImportFields(component.GetType(), component, componentJson.Value);
					ImportProperties(component.GetType(), component, componentJson.Value);
					
					component.Owner = node;
				
					node.Components[backingComponentType] = component;
				}
			}
			
			if(node.Root.QueryChild(node.Id) is null) {
				node.Root.Add(node);
			}
		}
		
		private static void ImportFields(this Type type, object inst, JsonNode json) {
			foreach(var field in type.GetFields()) {
				if(!field.IsForPublicNodeUse()) continue;
				
				ImportProperty(
					field.FieldType,
					json["fields"][field.Name],
					v => {
						if(v is decimal) {
							v = v.ReverseCommonDecimalConversion(field.FieldType);
						}
						
						field.SetValue(inst, v);
					}
				);
			}
		}

		private static void ImportProperties(this Type type, object inst, JsonNode json) {
			foreach(var property in type.GetProperties()) {
				if(!property.IsForPublicNodeUse()) continue;
				
				ImportProperty(
					property.PropertyType,
					json["properties"][property.Name],
					v => {
						if(v is decimal) {
							v = v.ReverseCommonDecimalConversion(property.PropertyType);
						}
						
						property.SetValue(inst, v);
					}
				);
			}
		}
		
		private static void ImportProperty(Type type, JsonNode? json, Action<object> setValue) {
			if(json is null) {
				setValue.Invoke(null);
				return;
			}
			
			var pType = type.GetCommonTypeForNodeUse();
			
			Tests.Assert(PROPERTY_IMPORTERS.TryGetValue(pType, out var action),
			             $"No importer for {pType}");
			
			var newValue =  action.Invoke(json);
			setValue.Invoke(newValue);
		}
	}
}