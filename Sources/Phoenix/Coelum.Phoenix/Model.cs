using System.Numerics;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;

namespace Coelum.Phoenix {
	
	public class Model {

		public string Name { get; }

		public List<Mesh> Meshes { get; init; } = new();
		public List<Material> Materials { get; init; } = new() { new() };

		public long VertexCount {
			get {
				long c = 0;
				
				foreach(var mesh in Meshes) {
					c += mesh.VertexCount;
				}

				return c;
			}
		}

		public Model(string name) {
			Name = name;
		}
		
		public virtual void Render(Matrix4x4 matrix, ShaderProgram shader) {
			shader.SetUniform("model", matrix);
			
			foreach(var mesh in Meshes) {
				Materials[mesh.MaterialIndex].Load(shader);
				mesh.Render(shader);
			}
		}
	}
}