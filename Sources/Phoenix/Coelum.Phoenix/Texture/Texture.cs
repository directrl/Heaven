using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Texture {
	
	public class Texture<TSize> : IDisposable {

		public TextureTarget Target { get; }

		public uint Id { get; init; }
		public TSize Size { get; init; }

		protected Texture(TextureTarget target, TSize size) {
			Target = target;

			Id = Gl.GenTexture();
			Size = size;
		}

		public void Bind(int unit = 0) {
			Gl.ActiveTexture((TextureUnit) ((int) TextureUnit.Texture0 + unit));
			Gl.BindTexture(Target, Id);
		}

		public void Dispose() {
			GC.SuppressFinalize(this);
			Gl.DeleteTexture(Id);
		}
	}
}