using System.Numerics;
using Coeli.Debug;
using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Object {
	
	public abstract class ObjectBase : Model {

		public abstract Matrix4x4 ModelMatrix { get; }

		public unsafe override void Render(ShaderProgram shader) {
			shader.SetUniform("instanced", true);
			shader.SetUniform("model", ModelMatrix);
			/*var gl = GLManager.Current;
			
			var vbo = new VertexBufferObject(gl, BufferTargetARB.ArrayBuffer);
			vbo.Bind();

			var model = ModelMatrix;
			gl.BufferData(vbo.Target, (nuint) sizeof(Matrix4x4), &model.M11, GLEnum.StaticDraw);
			
			foreach(var mesh in Meshes) {
				Tests.Assert(mesh._gl == gl);
				
				mesh.VAO.Bind();

				var type = VertexAttribPointerType.Float;
				uint size = (uint) sizeof(Matrix4x4);
					
				gl.EnableVertexAttribArray(3);
				gl.VertexAttribPointer(3, 4, type, false, size, null);
				gl.EnableVertexAttribArray(4);
				gl.VertexAttribPointer(4, 4, type, false, size, sizeof(Vector4));
				gl.EnableVertexAttribArray(5);
				gl.VertexAttribPointer(5, 4, type, false, size, (2 * sizeof(Vector4)));
				gl.EnableVertexAttribArray(6);
				gl.VertexAttribPointer(6, 4, type, false, size, (3 * sizeof(Vector4)));
					
				gl.VertexAttribDivisor(3, 1);
				gl.VertexAttribDivisor(4, 1);
				gl.VertexAttribDivisor(5, 1);
				gl.VertexAttribDivisor(6, 1);
					
				gl.BindVertexArray(0);
				
				mesh.Render();
			}*/
			base.Render(shader);
		}
	}
}