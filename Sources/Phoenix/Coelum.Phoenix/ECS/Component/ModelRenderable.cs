using System.Text.Json;
using System.Text.Json.Nodes;
using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public class ModelRenderable : INodeComponent, Renderable {

		public Node? Owner { get; set; }
		public Model Model { get; set; }
		
		public ModelRenderable() { }
		
		public ModelRenderable(Model model) {
			Model = model;
		}

		public void Render(ShaderProgram shader) {
			Model.Render(shader);
		}

		public void Serialize(string name, Utf8JsonWriter writer) {
			writer.WriteStartObject(GetType().FullName);
			writer.WriteString("backing_type", name);
			{
				writer.WriteString("model", Model.Name);
			}
			writer.WriteEndObject();
		}
		
		public INodeComponent Deserialize(JsonNode node) {
			var modelName = node["model"].GetValue<string>();

			if(!ModelRegistry.TryGet(modelName, out var model)) {
				throw new Exception($"Model with name [{modelName}] could not be found in registry");
			}

			Model = model;
			return this;
		}
	}
}