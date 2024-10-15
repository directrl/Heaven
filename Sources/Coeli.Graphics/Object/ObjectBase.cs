using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Object {
	
	public abstract class ObjectBase : Model {

		public abstract Matrix4x4 ModelMatrix { get; }

		public unsafe override void Render(ShaderProgram shader) {
			shader.SetUniform("model", ModelMatrix);
			base.Render(shader);
		}
	}
}