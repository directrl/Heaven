using System.Numerics;
using Coeli.Graphics.OpenGL;

namespace Coeli.Graphics.Object {
	
	public abstract class ObjectBase : Model {

		public abstract Matrix4x4 ModelMatrix { get; }

		public virtual void Render(ShaderProgram shader) {
			shader.SetUniform("model", ModelMatrix);
			base.Render();
		}
	}
}