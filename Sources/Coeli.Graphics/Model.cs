using Silk.NET.OpenGL;

namespace Coeli.Graphics {
	
	public class Model {

		public Mesh[] Meshes { get; init; }

		public Model(params Mesh[] meshes) {
			Meshes = meshes;
		}

		public virtual void Render() {
			foreach(var mesh in Meshes) {
				mesh.Render();
			}
		}
	}
}