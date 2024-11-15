using Coelum.ECS.Serialization;

namespace Coelum.Phoenix.ECS {
	
	public static class PropertyImporters {

		static PropertyImporters() {
			NodeImporter.PROPERTY_IMPORTERS[typeof(Model)] = json => {
				var modelName = json["model"].GetValue<string>();

				if(!ModelRegistry.TryGet(modelName, out var model)) {
					throw new Exception($"Model with name [{modelName}] could not be found in registry");
				}
				
				// TODO fix this or whatever
				// model.Materials.Clear();
				//
				// var materialsJson = json["materials"].AsArray();
				// foreach(var materialJson in materialsJson) {
				// 	model.Materials.Add(new Material().Deserialize(materialJson));
				// }
				
				return model;
			};
		}
	}
}