using Coelum.Debug;
using Coelum.Node;

namespace Coelum.World.Components {
	
	public interface IEntity : INodeComponent {

		public ulong Id { get; }
		public object World { get; }
		
		public Type[] Components { get; }
		
		public bool AsComponent<TComponent>(out TComponent result) where TComponent : INodeComponent {
			if(this is TComponent) {
				result = (TComponent) this;
				return true;
			}

			result = default;
			return false;
		}
	}
}