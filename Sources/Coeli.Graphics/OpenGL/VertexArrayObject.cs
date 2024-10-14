using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public readonly struct VertexArrayObject : IDisposable {

		private readonly GL _gl;
		
		public uint Id { get; }

		public VertexArrayObject(GL gl) {
			this._gl = gl;
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