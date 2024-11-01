using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public interface Renderable : NodeComponent {
		
		public void Render(ShaderProgram shader);
	}
}