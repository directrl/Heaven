using System.Runtime.CompilerServices;
using Coelum.Common.Ecs;
using Coelum.Common.Ecs.Component;
using Coelum.Phoenix.Ecs.Component;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.Scene;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs.System {
	
	public class RenderSystem : IEcsSystem<PhoenixSceneBase> {

		public static System<Transform, RenderableModel> Create(World world, PhoenixSceneBase scene) {
			return world.System<Transform, RenderableModel>("Render")
			            .Kind(0)
			            .Cached()
			            .Each((ref Transform transform, ref RenderableModel renderable) => {
				            renderable.Model.Render(transform.Matrix, scene.PrimaryShader);
			            });
		}
	}
}