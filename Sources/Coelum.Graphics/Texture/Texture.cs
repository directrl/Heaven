using Silk.NET.OpenGL;

using static Coelum.Graphics.OpenGL.GLManager;

namespace Coelum.Graphics.Texture {
	
	public class Texture<TSize> : IDisposable {

		protected TextureTarget Target { get; }

		public uint Id { get; init; }
		public TSize Size { get; init; }

		protected Texture(TextureTarget target, TSize size) {
			Target = target;

			Id = gl.GenTexture();
			Size = size;

			Bind();
		}

		public virtual void Bind() {
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(Target, Id);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			GL.DeleteTexture(Id);
		}
	}
}