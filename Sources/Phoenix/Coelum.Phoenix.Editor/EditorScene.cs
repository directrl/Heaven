using Coelum.Common.Input;
using Coelum.Phoenix.Input;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor {
	
	public class EditorScene : PhoenixScene {
		
		public KeyBindings KeyBindings { get; }

		public EditorScene() : base("editor-main") {
			Render += delta => {
				HexaImGui.Begin();
				ImGui.ShowDemoWindow();
				HexaImGui.End();
			};

			// KeyBindings = new(Id);
			// this.SetupKeyBindings(KeyBindings);
		}

		public override void OnLoad(SilkWindow window) {
			base.OnLoad(window);
			
			HexaImGui.Setup(window);
		}
	}
}