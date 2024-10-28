using Flecs.NET.Core;

namespace Coelum.World {
	
	public interface IWorldContainer<TRenderable, TTickable> {
		
		public Flecs.NET.Core.World World { get; init; }
		
		public Query<TRenderable> RenderableQuery { get; }
		public Query<TTickable> TickableQuery { get; }
	}
}
