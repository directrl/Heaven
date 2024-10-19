using System.Numerics;
using Coelum.Graphics.Components;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Node {
	
	public abstract class SpatialNode : NodeBase, IRenderable, IShaderLoadable {

		public GL GL { get; } = GLManager.Current;

		public virtual Model? Model { get; init; }
		public abstract Matrix4x4 ModelMatrix { get; }

		public void Load(ShaderProgram shader) {
			if(Model == null) return;
			
			Model.Load(shader);
			shader.SetUniform("model", ModelMatrix);
		}
		
		public void Render() {
			Model?.Render();
		}
	}
}