using Silk.NET.OpenGL;

using static Coelum.Graphics.OpenGL.GLManager;

namespace Coelum.Graphics.Texture {
	
	public class Texture<TSize> : IDisposable {

		protected TextureTarget Target { get; }

		public uint Id { get; init; }
		public TSize Size { get; init; }

		protected Texture(TextureTarget target, TSize size) {
			Target = target;

			Id = Gl.GenTexture();
			Size = size;

			Bind();
		}

		public virtual void Bind() {
			Gl.ActiveTexture(TextureUnit.Texture0);
			Gl.BindTexture(Target, Id);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Gl.DeleteTexture(Id);
		}
	}
}