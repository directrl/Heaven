using Coelum.Node;
using Coelum.World.Components;
using Coelum.World.Entity;

namespace Coelum.World.Queries {
	
	public static class EntityQueries {
		
		public static WorldEntity QueryById(World world, ulong id) {
			return world.Entities[id];
		}

		public static void QueryByComponentAll<TComponent>(World world,
		                                                   Action<WorldEntity>? forAll = null) 
			where TComponent : INodeComponent {
			
			Parallel.ForEach(world.Entities.Values, entity => {
				if(entity is TComponent) {
					forAll?.Invoke(entity);
				}
			});
		}
	}
}