using Silk.NET.OpenGL;

namespace Coeli.Graphics.OpenGL {
	
	public readonly struct VertexBufferObject : IDisposable {

		private readonly GL _gl;
		
		public uint Id { get; }
		public BufferTargetARB Target { get; }
		
		public VertexBufferObject(GL gl, BufferTargetARB target) {
			_gl = gl;
			Id = gl.GenBuffer();
			Target = target;
		}

		public void Bind() {
			_gl.BindBuffer(Target, Id);
		}

		public unsafe void Data<TDataType>(Span<TDataType> data, BufferUsageARB usage) {
			fixed(void* d = data) {
				_gl.BufferData(Target, (nuint) (data.Length * sizeof(TDataType)), d, usage);
			}
		}
		
		public void Dispose() {
			_gl.DeleteBuffer(Id);
		}
	}
}