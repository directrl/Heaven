using Silk.NET.OpenGL;

namespace Coelum.Graphics.OpenGL {
	
	public readonly struct VertexArrayObject : IDisposable {

		private readonly GL _gl;
		
		public uint Id { get; }

		public VertexArrayObject(GL gl) {
			_gl = gl;
			Id = gl.GenVertexArray();
		}

		public void Bind() {
			_gl.BindVertexArray(Id);
		}

		public void Dispose() {
			_gl.DeleteVertexArray(Id);
		}
	}
}