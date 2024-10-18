using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics {
	
	public class Model : IShaderRenderable {

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

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}