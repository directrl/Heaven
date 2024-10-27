using System.Drawing;
using Silk.NET.Maths;

namespace Coelum.Raven.Node {
	
	public class LabelNode : SpatialNode {
		
		public string Text { get; set; }
		public Color ForegroundColor { get; set; } = Color.White;
		public Color BackgroundColor { get; set; } = Color.Black;

		public bool Wrap { get; set; }
		public Vector2D<float> Anchor { get; set; }
		
		public LabelNode(string text, bool wrap = false) {
			Text = text;
			Wrap = wrap;
		}
		
		public override void Render(RenderContext ctx) {
			int x = GlobalPosition.X - (int) Math.Round(Anchor.X * Text.Length);
			int y = GlobalPosition.Y - (int) Math.Round(Anchor.Y * 1);
			
			for(int i = 0; i < Text.Length; i++) {
				if(Text[i] == '\n') {
					x = 0;
					y++;
					continue;
				}
				
				ctx[x, y] = new() {
					Character = Text[i],
					ForegroundColor = ForegroundColor,
					BackgroundColor = BackgroundColor
				};

				x++;

				if(x > (ctx.Display.Width - 1) && Wrap) {
					x = 0;
					y++;
				}
			}
		}
	}
}