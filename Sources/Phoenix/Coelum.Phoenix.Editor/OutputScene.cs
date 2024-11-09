using Coelum.Common.Input;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.Editor.Camera;
using Coelum.Phoenix.Input;
using Coelum.Phoenix.UI;

namespace Coelum.Phoenix.Editor {
	
	public class OutputScene : PhoenixScene {

		//private DebugUI _debugOverlay;
		
		public KeyBindings KeyBindings { get; }
		public FreeCamera3D FreeCamera { get; private set; }

		public OutputScene(string name) : base(name) {
			KeyBindings = new(name);
			this.SetupKeyBindings(KeyBindings);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);

			FreeCamera = new(new PerspectiveCamera(window), this, KeyBindings);
			FreeCamera.Camera.Current = true;
		}

		public override void OnUpdate(float delta) {
			base.OnUpdate(delta);
			this.UpdateKeyBindings(KeyBindings);
		}
	}
}