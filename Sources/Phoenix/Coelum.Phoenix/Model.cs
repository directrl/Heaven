using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix {
	
	public class Model {

		public string Name { get; init; }
		public List<Mesh> Meshes { get; init; }

		public Model(string name, Mesh[] meshes)
			: this(name, meshes.ToList()) { }
		public Model(string name, List<Mesh> meshes) {
			Name = name;
			Meshes = meshes;
		}
		
		public virtual void Render(ShaderProgram shader) {
			foreach(var mesh in Meshes) {
				mesh.Render(shader);
			}
		}
	}
}