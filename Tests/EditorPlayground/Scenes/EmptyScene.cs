using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.Lighting;
using Coelum.Phoenix.UI;

namespace EditorPlayground.Scenes {
	
	public class EmptyScene : PhoenixScene {

		public EmptyScene() : base("empty") {
			PrimaryShader.AddOverlays(Material.OVERLAYS);
			PrimaryShader.AddOverlays(SceneEnvironment.OVERLAYS);
			PrimaryShader.AddOverlays(PhongShading.OVERLAYS);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			//_ = new DebugUI(this);
		}
	}
}