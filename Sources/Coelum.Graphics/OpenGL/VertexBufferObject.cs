using Silk.NET.OpenGL;

namespace Coelum.Graphics.OpenGL {
	
	public readonly struct VertexBufferObject : IDisposable {
		
		public uint Id { get; }
		public BufferTargetARB Target { get; }
		
		public VertexBufferObject(BufferTargetARB target) {
			Id = Gl.GenBuffer();
			Target = target;
		}

		public void Bind() {
			Gl.BindBuffer(Target, Id);
		}

		public unsafe void Data<TDataType>(Span<TDataType> data, BufferUsageARB usage) {
			fixed(void* d = data) {
				Gl.BufferData(Target, (nuint) (data.Length * sizeof(TDataType)), d, usage);
			}
		}
		
		public void Dispose() {
			Gl.DeleteBuffer(Id);
		}
	}
}