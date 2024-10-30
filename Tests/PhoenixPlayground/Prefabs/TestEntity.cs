using Coelum.Common.Ecs;
using Coelum.Common.Ecs.Component;
using Coelum.Phoenix;
using Coelum.Phoenix.Components;
using Coelum.Phoenix.Ecs.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using Flecs.NET.Core;
using Tickable = Coelum.Common.Ecs.Component.Tickable;

namespace PhoenixPlayground.Prefabs {
	
	public class TestEntity : IPrefab {

		public void Setup(World world) {
			var model = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"]);
			
			world.Prefab<TestEntity>()
			     .Set<RenderableModel>(new(model))
			     .Set<Transform>(new Transform3D(scale: new(0.1f, 0.1f, 0.1f)));
		}
		
		public Entity Create(World world) {
			return world.Entity()
			            .IsA<TestEntity>();
		}
	}
}