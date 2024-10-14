using Coeli.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coeli.Graphics {
	
	public class Mesh : IDisposable {

		private readonly GL _gl;
		private readonly List<VertexBufferObject> _vbos = new();

		public readonly uint VertexCount;
		public readonly VertexArrayObject VAO;

		public readonly PrimitiveType Type;
		
		public unsafe Mesh(PrimitiveType type,
		            float[] vertices, uint[] indices, float[]? texCoords, float[]? normals) {
			
			_gl = GLManager.Current;
			Type = type;
			
			VertexCount = (uint) indices.Length;

			VAO = new(_gl);
			VAO.Bind();

		#region Vertices
			{
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(vertices, BufferUsageARB.StaticDraw);
				
				_gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
				_gl.EnableVertexAttribArray(0);
			}
		#endregion
			
		#region Texture Coordinates
			if(texCoords != null) {
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(texCoords, BufferUsageARB.StaticDraw);
			
				_gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
				_gl.EnableVertexAttribArray(1);
			}
		#endregion
			
		#region Normals
			if(normals != null) {
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(normals, BufferUsageARB.StaticRead);
			
				_gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
				_gl.EnableVertexAttribArray(2);
			}
		#endregion
			
		#region Indices
			{
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ElementArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<uint>(indices, BufferUsageARB.StaticDraw);
			}
		#endregion
			
			_gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			_gl.BindVertexArray(0);
		}

		public unsafe void Render() {
			VAO.Bind();
			_gl.DrawElements(Type, VertexCount, DrawElementsType.UnsignedInt, null);
		}

		public void Dispose() {
			foreach(var vbo in _vbos) {
				vbo.Dispose();
			}
			
			VAO.Dispose();
		}
	}
}