using Coelum.Graphics.Phoenix.OpenGL;

namespace Coelum.Graphics.Phoenix.Node.Component {
	
	public interface IShaderRenderable : IRenderable, IShaderLoadable {

		public void Render(ShaderProgram shader);
	}
}