using Coelum.Node;
using Coelum.Raven.Node.Component;
using Silk.NET.Maths;

namespace Coelum.Raven.Node {
	
	public abstract class SpatialNode : NodeBase, IRenderable {

		public Vector2D<int> Position = new();
		
		public virtual void Render(RenderContext ctx) {
			throw new NotImplementedException();
		}
	}
}