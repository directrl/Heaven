using Coelum.Node;
using Coelum.Raven.Node.Component;
using Silk.NET.Maths;

namespace Coelum.Raven.Node {
	
	public abstract class SpatialNode : NodeBase, ISpatial, IRenderable {

		public Vector2D<int> Position;
		public Vector2D<int> LocalPosition {
			get => Position;
			set => Position = value;
		}

		public virtual Vector2D<int> GlobalPosition {
			get {
				if(Parent is SpatialNode n) {
					return Position + n.Position;
				}

				return Position;
			}
		}

		public virtual void Render(RenderContext ctx) { }
	}
}