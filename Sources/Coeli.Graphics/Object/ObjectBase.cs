using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Object {
	
	public abstract class ObjectBase : Model {

		public abstract Matrix4x4 ModelMatrix { get; }

		public override void Load(ShaderProgram shader) {
			base.Load(shader);
			shader.SetUniform("model", ModelMatrix);
		}
	}
}