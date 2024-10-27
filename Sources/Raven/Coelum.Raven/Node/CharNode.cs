using System.Drawing;

namespace Coelum.Raven.Node {
	
	public class CharNode : SpatialNode {

		public char Character { get; set; }
		public Color ForegroundColor { get; set; } = Color.White;
		public Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
		
		public CharNode(char value) {
			Character = value;
		}
		
		public override void Render(RenderContext ctx) {
			ctx[GlobalPosition] = new() {
				Character = Character,
				ForegroundColor = ForegroundColor,
				BackgroundColor = BackgroundColor
			};
		}
	}
}