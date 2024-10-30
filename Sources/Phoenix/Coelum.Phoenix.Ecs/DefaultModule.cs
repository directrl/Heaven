global using FlEcs = Flecs.NET.Core.Ecs;

using Coelum.Common.Ecs;
using Coelum.Common.Ecs.Component;
using Coelum.Common.Ecs.System;
using Coelum.Phoenix.Ecs.Component;
using Coelum.Phoenix.Ecs.System;
using Coelum.Phoenix.Scene;
using Flecs.NET.Bindings;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs {
	
	public class DefaultModule : IEcsModule<PhoenixSceneBase> {

		public void InitModule(World world) {
			world.Module<DefaultModule>();

		#region Components
			world.Component<RenderableModel>();
			
			world.Component<Transform>();
			world.Component<Transform2D>();
			world.Component<Transform3D>();
		#endregion
		}

		public void Setup(World world, PhoenixSceneBase scene) {
			var sys = RenderSystem.Create(world, scene);
			scene.Render += sys.Run;

			TransformSystem.Create(world);
			TickSystem.Create(world);
		}
	}
}