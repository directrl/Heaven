using Coelum.Common.Ecs.Component;
using Flecs.NET.Core;

using FlEcs = Flecs.NET.Core.Ecs;

namespace Coelum.Common.Ecs.System {
	
	public class TickSystem : IEcsSystem {
		
		public static System<Tickable> Create(World world) {
			return world.System<Tickable>("Tick")
			            .Kind(FlEcs.OnUpdate)
			            .Cached()
			            .Each((Iter it, int i, ref Tickable tickable) => {
				            tickable.OnUpdate.Invoke(it.DeltaTime());
			            });
		}
	}
}