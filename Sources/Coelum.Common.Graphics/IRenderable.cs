using Coelum.Node;

namespace Coelum.Common.Graphics {
	
	public interface IRenderable : INodeComponent {
		
		public void Render(float delta);
	}
}