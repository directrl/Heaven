using Coelum.Graphics.Node;
using Coelum.Node;
using Coelum.World.Components;

namespace Coelum.World.Entity {
	
	public abstract class WorldEntity : Node3D {

		public ulong Id { get; private set; }
		public object World { get; private set; }

		public abstract Type[] Components { get; }
		
		public void Spawn(World world) {
			Id = world.CurrentEntityId;
			World = world;

			world.Entities[Id] = this;
		}
	}
}