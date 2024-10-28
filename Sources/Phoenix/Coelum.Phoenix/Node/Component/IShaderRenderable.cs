using Coelum.Node;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Node.Component {
	
	public interface IShaderRenderable : INodeComponent {

		public void Render(ShaderProgram shader);
	}
}