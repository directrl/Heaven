using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public interface IPrefab {

		public void Setup(World world);
		public Entity Create(World world);
	}
}