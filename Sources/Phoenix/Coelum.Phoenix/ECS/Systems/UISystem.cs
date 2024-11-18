using Coelum.ECS;
using Coelum.Phoenix.UI;
using Serilog;

namespace Coelum.Phoenix.ECS.Systems {
	
	// TODO should UI elements/overlays be 2D nodes?
	public class UISystem : EcsSystem {

		public override string Name => "UI Render & Update";
		public override SystemPhase Phase => SystemPhase.RENDER_POST;

		public UISystem() {
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			if(root is not PhoenixScene scene) {
				Log.Warning($"Root [{root.GetType().Name}] is not of type PhoenixScene! UI rendering is not possible");
				return;
			}

			var imController = ImGuiManager.CreateController(scene);
			
			imController.MakeCurrent();
			imController.Update(delta);
			
			foreach(var overlay in scene.UIOverlays) {
				if(!overlay.Visible) continue;
				overlay.Render(delta);
			}
			
			imController.Render();
		}
	}
}