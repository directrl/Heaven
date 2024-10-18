using Coelum.Node;
using Coelum.World.Components;
using Coelum.World.Entity;

namespace Coelum.World.Queries {
	
	public static class ComponentQueries {
		
		public static void QueryByComponentAll<TEntity, TComponent>(World<TEntity> world,
		                                                                         Action<TComponent>? forAll = null) 
			where TEntity : IEntity
			where TComponent : INodeComponent {
			
			Parallel.ForEach(world.Entities.Values, entity => {
				if(entity is TComponent component) {
					forAll?.Invoke(component);
				}
			});
		}
	}
}