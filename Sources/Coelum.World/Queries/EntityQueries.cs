using Coelum.Node;
using Coelum.World.Components;
using Coelum.World.Entity;

namespace Coelum.World.Queries {
	
	public static class EntityQueries {
		
		public static TEntity QueryById<TEntity>(World<TEntity> world, ulong id)
			where TEntity : IEntity {
			
			return world.Entities[id];
		}

		public static void QueryByComponentAll<TEntity, TComponent>(World<TEntity> world,
		                                                                     Action<TEntity>? forAll = null) 
			where TEntity : IEntity
			where TComponent : INodeComponent {
			
			Parallel.ForEach(world.Entities.Values, entity => {
				if(entity is TComponent) {
					forAll?.Invoke(entity);
				}
			});
		}
	}
}