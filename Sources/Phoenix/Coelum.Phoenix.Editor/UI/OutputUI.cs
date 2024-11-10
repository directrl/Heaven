using System.Numerics;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;
using Hexa.NET.ImGuizmo;

namespace Coelum.Phoenix.Editor.UI {
	
	public class OutputUI : ImGuiUI {

		private readonly OutputScene _output;
		private Vector2 _lastSize;

		public OutputUI(PhoenixScene scene, OutputScene output) : base(scene) {
			_output = output;
		}

		public override void Render(float delta) {
			if(_output._editor) {
				ImGuizmo.SetImGuiContext(ImGuiManager.CreateController(Scene).Context);
				ImGuizmo.SetDrawlist(ImGui.GetWindowDrawList());
				ImGuizmo.BeginFrame();
				ImGuizmo.Enable(true);
				// TODO how do you use imguizmo
			}
			
			ImGui.Begin(_output.Id);
			{
				var size = ImGui.GetContentRegionAvail();

				if(size != _lastSize && size is { X: > 0, Y: > 0 }) {
					_output.OutputFramebuffer.Size = new((int) size.X, (int) size.Y);
					_lastSize = size;
				}
				
				ImGui.Image(
					new ImTextureID(_output.OutputFramebuffer.Texture.Id),
					size,
					new Vector2(0, 1), new Vector2(1, 0)
				);
			}
			ImGui.End();

			if(_output._editor) {
				//ImGuizmo.Enable(false);
			}
		}
	}
}