using Coelum.Phoenix;
using Coelum.Phoenix.Components;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using Coelum.World.Components;
using Flecs.NET.Core;

namespace PhoenixPlayground.Prefabs {
	
	public class TestEntity {

		public static readonly Model MODEL;

		static TestEntity() {
			MODEL = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"]);
		}

		public static void AddPrefab(World world) {
			world.Prefab<TestEntity>()
			     .Set<Position3D>(new(0, 0, 0))
			     .Set<Rotation3D>(new(0, 0, 0))
			     .Set<Scale3D>(new(0, 0, 0))
			     .Add<Renderable>();
		}
		
		public static Entity Create(World world) {
			return world.Entity()
			            .IsA<TestEntity>()
			            .Set<Renderable>(new((delta, shader) => {
				            MODEL.Render(shader);
			            }));
		}
	}
}