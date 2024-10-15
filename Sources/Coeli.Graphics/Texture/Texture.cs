using Silk.NET.OpenGL;

namespace Coeli.Graphics.Texture {
	
	public class Texture<TSize> : IDisposable {

		private readonly GL _gl;
		private readonly TextureTarget _target;

		public uint Id { get; init; }
		public TSize Size { get; init; }

		public Texture(GL gl, TextureTarget target, TSize size) {
			_gl = gl;
			_target = target;

			Id = gl.GenTexture();
			Size = size;

			Bind();
		}

		public void Bind() {
			_gl.ActiveTexture(TextureUnit.Texture0);
			_gl.BindTexture(_target, Id);
		}

		public void Dispose() {
			_gl.DeleteTexture(Id);
		}
	}
}