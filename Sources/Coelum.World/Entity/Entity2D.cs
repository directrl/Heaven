using Coelum.Graphics.Node;
using Coelum.World.Components;

namespace Coelum.World.Entity {
	
	public abstract class Entity2D : Node2D, IEntity {

		public ulong Id { get; private set; }
		public object World { get; private set; }
		
		public abstract Type[] Components { get; }
		
		public void Spawn(World<Entity2D> world) {
			Id = world.CurrentEntityId;
			World = world;

			world.Entities[Id] = this;
		}
	}
}