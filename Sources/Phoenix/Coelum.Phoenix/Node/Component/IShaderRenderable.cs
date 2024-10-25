using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Node.Component {
	
	public interface IShaderRenderable : IRenderable, IShaderLoadable {

		public void Render(ShaderProgram shader);
	}
}