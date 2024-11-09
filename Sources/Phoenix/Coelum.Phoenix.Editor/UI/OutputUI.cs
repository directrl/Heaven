using System.Numerics;
using Coelum.Phoenix.UI;
using Hexa.NET.ImGui;

namespace Coelum.Phoenix.Editor.UI {
	
	public class OutputUI : ImGuiOverlay {

		private OutputScene _output;
		private Vector2 _lastSize;

		public OutputUI(PhoenixScene scene, OutputScene output, bool editor = true) : base(scene) {
			_output = output;
			
			Render += (delta, args) => {
				if(ImGui.Begin(editor ? "Scene" : "Output")) {
					var size = ImGui.GetContentRegionAvail();

					if(size != _lastSize && size is { X: > 0, Y: > 0 }) {
						_output.OutputFramebuffer = new((uint) size.X, (uint) size.Y);
						_lastSize = size;
					}
					
					ImGui.Image(
						new ImTextureID(_output.OutputFramebuffer.Texture.Id),
						size,
						new Vector2(0, 1), new Vector2(1, 0)
					);
					
					ImGui.End();
				}
			};
		}
	}
}