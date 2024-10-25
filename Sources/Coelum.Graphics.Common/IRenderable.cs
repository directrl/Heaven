using Coelum.Node;

namespace Coelum.Graphics.Common {
	
	public interface IRenderable : INodeComponent {
		
		public void Render(float delta);
	}
}