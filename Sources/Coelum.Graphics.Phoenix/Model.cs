using Coelum.Graphics.Phoenix.OpenGL;

namespace Coelum.Graphics.Phoenix {
	
	public class Model {

		public string Name { get; init; }
		
		public List<Mesh> Meshes { get; init; }
		
		[Obsolete]
		public Material Material { get; set; } = new();

		[Obsolete]
		public Model(List<Mesh> meshes) {
			Meshes = meshes;
		}
		
		[Obsolete]
		public Model(params Mesh[] meshes) {
			Meshes = meshes.ToList();
		}

		public Model(string name, Mesh[] meshes)
			: this(name, meshes.ToList()) { }
		public Model(string name, List<Mesh> meshes) {
			Name = name;
			Meshes = meshes;
		}

		[Obsolete]
		public virtual void Load(ShaderProgram shader) {
			Material.Load(shader);
		}
		
		public virtual void Render(ShaderProgram shader) {
			foreach(var mesh in Meshes) {
				mesh.Render(shader);
			}
		}
	}
}