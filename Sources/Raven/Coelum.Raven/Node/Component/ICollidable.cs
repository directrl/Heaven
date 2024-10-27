using Coelum.Node;
using Coelum.Physics.Collision;

namespace Coelum.Raven.Node.Component {
	
	public interface ICollidable : INodeComponent, ISpatial {
		
		public BoundingBox2D<int> AABB { get; }

		public BoundingBox2D<int> RelativeAABB => new(
			new(GlobalPosition.X + AABB.Min.X, GlobalPosition.Y + AABB.Min.Y),
			new(GlobalPosition.X + AABB.Max.X, GlobalPosition.Y + AABB.Max.Y)
		);
	}
}