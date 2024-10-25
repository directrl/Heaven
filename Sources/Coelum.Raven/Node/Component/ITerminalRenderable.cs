using Coelum.Node;
using Coelum.Raven.Terminal;

namespace Coelum.Raven.Node.Component {
	
	public interface ITerminalRenderable : INodeComponent {

		public void Render(TerminalBase terminal);
	}
}