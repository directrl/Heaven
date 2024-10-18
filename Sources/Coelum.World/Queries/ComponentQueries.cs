using Coelum.Node;
using Coelum.World.Components;
using Coelum.World.Entity;

namespace Coelum.World.Queries {
	
	public static class ComponentQueries {
		
		public static void QueryByComponentAll<TComponent>(World world,
		                                                   Action<TComponent>? forAll = null) 
			where TComponent : INodeComponent {
			
			Parallel.ForEach(world.Entities.Values, entity => {
				if(entity is TComponent component) {
					forAll?.Invoke(component);
				}
			});
		}
	}
}