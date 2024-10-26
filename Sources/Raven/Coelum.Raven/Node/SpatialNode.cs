using Coelum.Node;
using Coelum.Raven.Node.Component;
using Coelum.Raven.Terminal;
using Silk.NET.Maths;

namespace Coelum.Raven.Node {
	
	public abstract class SpatialNode : NodeBase, ITerminalRenderable {

		public Vector2D<int> Position = new();
		
		public virtual void Render(TerminalBase terminal) {
			throw new NotImplementedException();
		}
	}
}