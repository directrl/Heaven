using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics {
	
	public class Model {

		public Mesh[] Meshes { get; init; }
		public Material Material { get; set; } = new();

		public Model(params Mesh[] meshes) {
			Meshes = meshes;
		}

		public virtual void Load(ShaderProgram shader) {
			Material.Load(shader);
		}
		
		public virtual void Render() {
			foreach(var mesh in Meshes) {
				mesh.Render();
			}
		}
	}
}