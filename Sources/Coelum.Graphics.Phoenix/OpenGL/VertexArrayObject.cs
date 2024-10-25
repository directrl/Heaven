using Silk.NET.OpenGL;

namespace Coelum.Graphics.Phoenix.OpenGL {
	
	public readonly struct VertexArrayObject : IDisposable {
		
		public uint Id { get; }

		public VertexArrayObject() {
			Id = Gl.GenVertexArray();
		}

		public void Bind() {
			Gl.BindVertexArray(Id);
		}

		public void Dispose() {
			Gl.DeleteVertexArray(Id);
		}
	}
}