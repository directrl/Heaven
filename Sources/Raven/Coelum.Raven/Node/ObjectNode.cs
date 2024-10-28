using Silk.NET.Maths;

namespace Coelum.Raven.Node {
	
	public class ObjectNode : SpatialNode {

		public Cell[,] Cells { get; }
		public Vector2D<float> Anchor { get; set; }

		public ObjectNode(Cell[,] cells) {
			Cells = cells;
		}

		public override void Render(RenderContext ctx) {
			int x = GlobalPosition.X - (int) Math.Round(Anchor.X * Cells.GetLength(0));
			int y = GlobalPosition.Y - (int) Math.Round(Anchor.Y * Cells.GetLength(1));

			for(int iy = 0; iy < Cells.GetLength(1); iy++) {
				for(int ix = 0; ix < Cells.GetLength(0); ix++) {
					var cell = Cells[iy, ix];
					ctx[x + ix, y + iy] = cell;
				}
			}
		}
	}
}