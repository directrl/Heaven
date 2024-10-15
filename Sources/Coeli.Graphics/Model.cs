using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics {
	
	public class Model {

		public Mesh[] Meshes { get; init; }
		public Material Material { get; init; } = Material.DEFAULT_MATERIAL;

		public Model(params Mesh[] meshes) {
			Meshes = meshes;
		}

		public virtual void Render(ShaderProgram shader) {
			Material.Load(shader);
			
			foreach(var mesh in Meshes) {
				mesh.Render();
			}
		}
	}
}