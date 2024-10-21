using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Node.Component {
	
	public interface IRenderable : INodeComponent {
		
		public void Render();
	}
}