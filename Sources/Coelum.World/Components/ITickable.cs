using Coelum.Node;

namespace Coelum.World.Components {
	
	public interface ITickable : INodeComponent {

		public void Update(float delta);
	}
}