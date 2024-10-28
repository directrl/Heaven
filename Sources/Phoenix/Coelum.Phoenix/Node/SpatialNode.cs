using System.Numerics;
using Coelum.Node;
using Coelum.Phoenix.Node.Component;
using Coelum.Phoenix.OpenGL;
using Silk.NET.OpenGL;
using Shader = Coelum.Phoenix.OpenGL.Shader;

namespace Coelum.Phoenix.Node {
	
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
			
			shader.SetUniform("model", GlobalTransform);
			Model?.Render(shader);
		}
	}
}