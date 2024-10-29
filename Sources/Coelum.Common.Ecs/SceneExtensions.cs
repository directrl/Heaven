using Coelum.Common.Graphics;
using Coelum.Debug;
using Flecs.NET.Bindings;
using Flecs.NET.Core;

using FlEcs = Flecs.NET.Core.Ecs;

namespace Coelum.Common.Ecs {
	
	public static class SceneExtensions {
		
		public static World CreateWorld(this SceneBase scene) {
			var world = World.Create();
			world.SetThreads(Environment.ProcessorCount);

			if(Debugging.Enabled) {
				world.Import<FlEcs.Units>();
				world.Import<FlEcs.Stats>();
				world.Import<FlEcs.Metrics>();
				world.Import<FlEcs.Meta>();

				world.App().EnableRest();
				world.Set(default(flecs.EcsRest));
			}

			scene.Update += delta => world.Progress(delta);

			return world;
		}
	}
}