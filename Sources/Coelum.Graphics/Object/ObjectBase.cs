using System.Numerics;
using Coelum.Graphics.OpenGL;

namespace Coelum.Graphics.Object {
	
	public abstract class ObjectBase : Model {

		public abstract Matrix4x4 ModelMatrix { get; }

		public override void Load(ShaderProgram shader) {
			base.Load(shader);
			shader.SetUniform("model", ModelMatrix);
		}
	}
}