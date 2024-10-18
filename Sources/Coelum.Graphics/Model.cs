using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics {
	
	public class Model : _IShaderRenderable {

		public Mesh[] Meshes { get; init; }
		public Material Material = new();

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

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}