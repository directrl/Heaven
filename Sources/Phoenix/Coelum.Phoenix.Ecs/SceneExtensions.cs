using Coelum.Common.Ecs;
using Coelum.Common.Graphics;
using Coelum.Configuration;
using Coelum.Debug;
using Coelum.Phoenix.Scene;
using Flecs.NET.Bindings;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs {
	
	public static class SceneExtensions {

		public static World CreateWorld(this PhoenixSceneBase scene) {
			var world = Common.Ecs.SceneExtensions.CreateWorld(scene);
			world.Import<DefaultModule, PhoenixSceneBase>(scene);
			
			return world;
		}
	}
}