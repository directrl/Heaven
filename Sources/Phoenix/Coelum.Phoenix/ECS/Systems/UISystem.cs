using Coelum.ECS;
using Coelum.Phoenix.UI;
using Serilog;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class UISystem : EcsSystem {

		public UISystem() : base("UI Render & Update") {
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