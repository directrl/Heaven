using Coelum.Node;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Component {
	
	public interface Renderable : NodeComponent {
		
		public void Render(ShaderProgram shader);
	}
}