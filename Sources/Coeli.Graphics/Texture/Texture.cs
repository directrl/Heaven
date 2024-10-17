using System.Numerics;
using Coeli.Graphics.OpenGL;
using Serilog;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Coeli.Graphics.Texture {
	
	public class Texture<TSize> : IDisposable {

		protected GL GL { get; }
		protected TextureTarget Target { get; }

		public uint Id { get; init; }
		public TSize Size { get; init; }

		protected Texture(GL gl, TextureTarget target, TSize size) {
			GL = gl;
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