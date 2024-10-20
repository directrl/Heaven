using System.Numerics;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Silk.NET.OpenGL;
using Shader = Coelum.Graphics.OpenGL.Shader;

namespace Coelum.Graphics.Node {
	
	public abstract class SpatialNode : NodeBase, Component.IShaderRenderable {

		public GL GL { get; } = GLManager.Current;

		public virtual Model? Model { get; init; }
		public abstract Matrix4x4 LocalTransform { get; }
		
		public virtual Matrix4x4 GlobalTransform {
			get {
				if(Parent is SpatialNode n) {
					return LocalTransform * n.GlobalTransform;
				}

				return LocalTransform;
			}
		}

		public virtual void Load(ShaderProgram shader) {
			if(Model == null) return;
			
			Model.Load(shader);
			shader.SetUniform("model", GlobalTransform);
		}
		
		public virtual void Render() {
			Model?.Render();
		}

		public void Render(ShaderProgram shader) {
			Load(shader);
			Render();
		}
	}
}