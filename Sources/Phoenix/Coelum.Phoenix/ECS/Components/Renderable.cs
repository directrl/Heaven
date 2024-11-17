using Coelum.ECS;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Components {
	
	public interface Renderable : INodeComponent {
		
		public void Render(ShaderProgram shader);
	}
}