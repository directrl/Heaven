using Coelum.Common.Input;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public abstract class ImGuiScene : PhoenixScene {

		private ImGuiOverlay _overlay;
		
		public KeyBindings KeyBindings { get; }

		protected ImGuiScene(string name) : base(name) {
			KeyBindings = new(Id);
			this.SetupKeyBindings(KeyBindings);
		}
		
		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			_overlay = new(this);
			_overlay.Render += (delta, args) => {
				RenderUI(delta);
			};
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			this.UpdateKeyBindings(KeyBindings);
		}

		public virtual void RenderUI(float delta) {
			ImGui.SetNextWindowPos(new(0, 0));
			ImGui.SetNextWindowSize(new(Window.FramebufferWidth, Window.FramebufferHeight));
		}
	}
}