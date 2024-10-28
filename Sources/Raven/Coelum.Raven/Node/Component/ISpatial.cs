using Coelum.Node;
using Silk.NET.Maths;

namespace Coelum.Raven.Node.Component {
	
	public interface ISpatial : INodeComponent {
		
		public Vector2D<int> LocalPosition { get; set; }
		public Vector2D<int> GlobalPosition { get; }
	}
}