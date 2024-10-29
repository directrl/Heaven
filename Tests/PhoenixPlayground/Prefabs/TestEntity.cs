using Coelum.Common.Ecs;
using Coelum.Common.Ecs.Component;
using Coelum.Phoenix;
using Coelum.Phoenix.Components;
using Coelum.Phoenix.Ecs.Component;
using Coelum.Phoenix.ModelLoading;
using Coelum.Resources;
using Flecs.NET.Core;

namespace PhoenixPlayground.Prefabs {
	
	public class TestEntity : IPrefab {

		public void Setup(World world) {
			var model = ModelLoader.Load(Playground.AppResources[ResourceType.MODEL, "untitled.glb"]);
			
			world.Prefab<TestEntity>()
			     .Set<RenderableModel>(new(model))
			     .Set<Transform>(new Transform3D());
		}
		
		public Entity Create(World world) {
			return world.Entity()
			            .IsA<TestEntity>()
			            /*.Set<Renderable>(new((delta, shader) => {
				            MODEL.Render(shader);
			            }))*/;
		}
	}
}