using Silk.NET.OpenGL;

using static Coelum.Phoenix.OpenGL.GLManager;

namespace Coelum.Phoenix.Texture {
	
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

		public virtual void Bind(int unit = 0) {
			Gl.ActiveTexture((TextureUnit) ((int) TextureUnit.Texture0 + unit));
			Gl.BindTexture(Target, Id);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Gl.DeleteTexture(Id);
		}
	}
}