using Coelum.World.Components;
using Coelum.World.Entity;

namespace Coelum.World {
	
	public class World {

		public Dictionary<ulong, WorldEntity> Entities { get; } = new();

		private ulong _currentEntityId = 0;
		public ulong CurrentEntityId => _currentEntityId++;
	}
}