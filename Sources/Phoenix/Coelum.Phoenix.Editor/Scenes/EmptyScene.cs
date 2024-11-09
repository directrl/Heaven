using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Lighting;

namespace Coelum.Phoenix.Editor.Scenes {
	
	public class EmptyScene : PhoenixScene {

		public EmptyScene() : base("empty") {
			ShaderOverlays = new[] {
				Material.OVERLAYS,
				SceneEnvironment.OVERLAYS,
				PhongShading.OVERLAYS
			};
		}
	}
}