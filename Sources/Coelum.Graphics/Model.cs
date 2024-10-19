using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics {
	
	public class Model : _IShaderRenderable, ICloneable {

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

		public object Clone() {
			var meshes = new Mesh[Meshes.Length];

			for(int i = 0; i < Meshes.Length; i++) {
				meshes[i] = (Mesh) Meshes[0].Clone();
			}

			return new Model(meshes) {
				Material = Material
			};
		}
	}
}