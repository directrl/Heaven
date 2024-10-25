using System.Numerics;
using Coelum.Graphics.Node.Component;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Silk.NET.OpenGL;
using Shader = Coelum.Graphics.OpenGL.Shader;

namespace Coelum.Graphics.Node {
	
	public abstract class SpatialNode : NodeBase, IShaderRenderable {

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

		public virtual void Render(ShaderProgram shader) {
			if(Model == null) return;
			
			//Model.Load(shader);
			shader.SetUniform("model", GlobalTransform);
			Model?.Render(shader);
		}
		
		[Obsolete]
		public virtual void Render() {
			
		}

		[Obsolete]
		public void Load(ShaderProgram shader) {
			
		}
	}
}