using Coelum.Graphics.OpenGL;
using Silk.NET.OpenGL;

namespace Coelum.Graphics {
	
	public class Mesh : IDisposable, ICloneable {

		internal readonly GL _gl;
		internal readonly List<VertexBufferObject> _vbos = new();

		private readonly float[] _vertices;
		private readonly uint[] _indices;
		private readonly float[]? _texCoords;
		private readonly float[]? _normals;
		
		public readonly uint VertexCount;
		public readonly VertexArrayObject VAO;

		public readonly PrimitiveType Type;
		
		public Mesh(PrimitiveType type,
		            float[] vertices, uint[] indices, float[]? texCoords, float[]? normals) {
			
			_gl = GLManager.Current;
			Type = type;

			_vertices = vertices;
			_indices = indices;
			_texCoords = texCoords;
			_normals = normals;
			
			VertexCount = (uint) indices.Length;

			VAO = new(_gl);
			VAO.Bind();

		#region Vertices
			{
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(vertices, BufferUsageARB.StaticDraw);
				
				_gl.EnableVertexAttribArray(0);
				_gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			}
		#endregion
			
		#region Texture Coordinates
			if(texCoords != null) {
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(texCoords, BufferUsageARB.StaticDraw);
			
				_gl.EnableVertexAttribArray(1);
				_gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
			}
		#endregion
			
		#region Normals
			if(normals != null) {
				var vbo = new VertexBufferObject(_gl, BufferTargetARB.ArrayBuffer);
				_vbos.Add(vbo);
			
				vbo.Bind();
				vbo.Data<float>(normals, BufferUsageARB.StaticRead);
			
				_gl.EnableVertexAttribArray(2);
				_gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
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

		public unsafe virtual void Render() {
			VAO.Bind();
			_gl.DrawElements(Type, VertexCount, DrawElementsType.UnsignedInt, null);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			
			foreach(var vbo in _vbos) {
				vbo.Dispose();
			}
			
			VAO.Dispose();
		}

		public object Clone() {
			return new Mesh(Type, _vertices, _indices, _texCoords, _normals);
		}
	}
}