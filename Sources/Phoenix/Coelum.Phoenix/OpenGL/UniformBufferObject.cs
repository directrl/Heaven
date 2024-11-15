using Coelum.Debug;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.OpenGL {
	
	public class UniformBufferObject : IDisposable {

		internal static int _lastBinding = 0;
		
		protected int Offset = 0;

		public string Name { get; }
		
		public uint Handle { get; }
		public BufferTargetARB Target { get; }
		
		public int Binding { get; }
		
		public UniformBufferObject(string name, BufferTargetARB target = BufferTargetARB.UniformBuffer) {
			Name = name;
			Handle = Gl.GenBuffer();
			Target = target;

			Binding = _lastBinding++;
		}

		public void Bind() {
			Gl.BindBuffer(Target, Handle);
		}

		public void Unbind() {
			Gl.BindBuffer(Target, 0);
		}

		public void BindRange(int index, int offset, int size) {
			Gl.BindBufferRange(Target, (uint) index, Handle, offset, (uint) size);
		}
		
		public unsafe void Allocate(int size, BufferUsageARB usage = BufferUsageARB.StaticDraw) {
			Gl.BufferData(Target, (uint) size, null, usage);
		}

		public unsafe void Data<TData>(int size, TData* data, BufferUsageARB usage = BufferUsageARB.StaticDraw) {
			Gl.BufferData(Target, (uint) size, data, usage);
		}

		public unsafe void SubData<TData>(int offset, int size, TData* data) {
			Gl.BufferSubData(Target, offset, (uint) size, data);
		}
		
		public unsafe void SubData<TData>(int offset, int size, TData data) {
			//fixed(TData* ptr = &data) {
				Gl.BufferSubData(Target, offset, (uint) size, &data);
			//}
		}

		public virtual void Upload() {
			Offset = 0;
		}
		
		protected unsafe void SubUpload<TData>(TData* data, int size = -1) {
			if(size == -1) size = sizeof(TData);
			SubData(Offset, size, data);
			Offset += size;
		}

		protected unsafe void SubUpload<TData>(TData data, int size = -1) {
			if(size == -1) size = sizeof(TData);
			SubData(Offset, size, data);
			Offset += size;
		}

		public void Dispose() {
			Gl.DeleteBuffer(Handle);
		}
	}

	public static class UBO_ShaderProgramExtensions {
		
		public static TUBO CreateBufferBinding<TUBO>(this ShaderProgram shader)
			where TUBO : UniformBufferObject, new() {
			
			if(shader.UBOs.ContainsKey(typeof(TUBO))) {
				return shader.GetUBO<TUBO>();
			}

			var ubo = new TUBO();
			
			uint index = Gl.GetUniformBlockIndex(shader.Id, ubo.Name);
			Gl.UniformBlockBinding(shader.Id, index, (uint) ubo.Binding);
			
			shader.AddUBO(ubo);
			return ubo;
		}
	}
}