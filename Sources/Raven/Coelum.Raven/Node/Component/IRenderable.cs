using Coelum.Node;

namespace Coelum.Raven.Node.Component {
	
	public interface IRenderable : INodeComponent {

		public void Render(RenderContext ctx);
	}
}