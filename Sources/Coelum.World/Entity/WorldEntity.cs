using Coelum.Graphics.Node;
using Coelum.Node;
using Coelum.World.Components;

namespace Coelum.World.Entity {
	
	public abstract class WorldEntity : Node3D {

		public ulong Id { get; internal set; }
		public World World { get; internal set; }

		public abstract Type[] Components { get; }
	}
}