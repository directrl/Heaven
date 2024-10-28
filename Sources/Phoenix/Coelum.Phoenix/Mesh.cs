using Coelum.Phoenix.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix {
	
	public class Mesh : IDisposable {

		internal readonly List<VertexBufferObject> _vbos = new();

		public readonly uint VertexCount;
		public readonly VertexArrayObject VAO;

		public PrimitiveType Type { get; }
		public int MaterialIndex { get; set; } = 0;
		
		public Mesh(PrimitiveType type,
		            float[] vertices, uint[] indices, float[]? texCoords, float[]? normals) {
			
			Type = type;
			
			VertexCount = (uint) indices.Length;

			VAO = new();
			VAO.Bind();

		#region Vertices
			{
				var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(vertices, BufferUsageARB.StaticDraw);
				
				Gl.EnableVertexAttribArray(0);
				Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			}
		#endregion
			
		#region Texture Coordinates
			if(texCoords != null) {
				var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(texCoords, BufferUsageARB.StaticDraw);
			
				Gl.EnableVertexAttribArray(1);
				Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			}
		#endregion
			
		#region Normals
			if(normals != null) {
				var vbo = new VertexBufferObject(BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(normals, BufferUsageARB.StaticRead);
			
				Gl.EnableVertexAttribArray(2);
				Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
			}
		#endregion
			
		#region Indices
			{
				var vbo = new VertexBufferObject(BufferTargetARB.ElementArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<uint>(indices, BufferUsageARB.StaticDraw);
			}
		#endregion
			
			Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
			Gl.BindVertexArray(0);
		}

		public unsafe virtual void Render(ShaderProgram shader) {
			VAO.Bind();
			Gl.DrawElements(Type, VertexCount, DrawElementsType.UnsignedInt, null);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			
			foreach(var vbo in _vbos) {
				vbo.Dispose();
			}
			
			VAO.Dispose();
		}
	}
}