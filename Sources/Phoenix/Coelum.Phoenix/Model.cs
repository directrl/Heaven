using Coelum.Debug;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Serilog;

namespace Coelum.Phoenix {
	
	public class Model {

		public string Name { get; }

		public List<Mesh> Meshes { get; init; } = new();
		public List<Material> Materials { get; init; } = new() { new() };

		public long VertexCount {
			get {
				long c = 0;
				
				foreach(var mesh in Meshes) {
					c += mesh.Vertices.Length;
				}

				return c;
			}
		}

		public Model(string? name) {
			if(name is null) {
				Name = "";
				return;
			}
			
			Name = name;

			if(!ModelRegistry.TryGet(Name, out _)) {
				ModelRegistry.Add(this);
				Log.Verbose($"Added model [{Name}] to registry");
			} else {
				Log.Warning($"Creating a model with name [{Name}] although it already exists in registry");
			}
		}

		/// <summary>
		/// Creates a(n optionally) deep copy of an existing model
		/// </summary>
		/// <param name="other">The model to create a copy of</param>
		/// <param name="deepMeshes">Whether or not to deeply copy meshes</param>
		/// <param name="deepMaterials">Whether or not to deeply copy materials</param>
		public Model(Model other, bool deepMeshes = false, bool deepMaterials = true) {
			Name = other.Name;

			if(deepMeshes) {
				foreach(var mesh in other.Meshes) {
					Meshes.Add(new(mesh));
				}
			} else {
				Meshes = new(other.Meshes);
			}

			if(deepMaterials) {
				Materials.Clear();

				foreach(var material in other.Materials) {
					Materials.Add(material);
				}
			} else {
				Materials = new(other.Materials);
			}
		}
		
		public virtual void Render(ShaderProgram shader) {
			foreach(var mesh in Meshes) {
				Materials[mesh.MaterialIndex].Load(shader);
				mesh.Render();
			}
		}
	}

	public static class ModelRegistry {

		public static readonly Dictionary<string, Model> REGISTRY = new();

		public static void Add(IResource resource, Model model) {
			REGISTRY.Add(resource.UID, model);
		}

		public static void Add(string name, Model model) {
			REGISTRY.Add(name, model);
		}

		public static void Add(Model model) {
			Tests.Assert(!string.IsNullOrWhiteSpace(model.Name), "model.Name cannot be empty or null");
			REGISTRY.Add(model.Name, model);
		}

		public static Model Get(IResource resource) {
			return REGISTRY[resource.UID];
		}

		public static Model Get(string name) {
			return REGISTRY[name];
		}

		public static bool TryGet(IResource resource, out Model model) {
			if(REGISTRY.TryGetValue(resource.UID, out model)) {
				return true;
			}

			return false;
		}
		
		public static bool TryGet(string name, out Model model) {
			if(REGISTRY.TryGetValue(name, out model)) {
				return true;
			}

			return false;
		}
	}
}