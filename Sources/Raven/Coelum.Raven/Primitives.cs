using System.Drawing;
using Coelum.Physics.Collision;
using Coelum.Raven.Node;
using Coelum.Raven.Node.Component;

namespace Coelum.Raven {
	
	public static class Primitives {

		public static readonly Cell BOUNDING_BOX_LINE = new('▖', Color.DarkCyan);
		
		public static void DrawLine(this RenderContext ctx, int x1, int y1, int x2, int y2, Cell value) {
			int dx = Math.Abs(x2 - x1);
			int dy = Math.Abs(y2 - y1);
			int sx = x1 < x2 ? 1 : -1;
			int sy = y1 < y2 ? 1 : -1;
			int err = dx - dy;

			while(true) {
				ctx[x1, y1] = value;

				if(x1 == x2 && y1 == y2) {
					break;
				}

				int e2 = 2 * err;
				
				if(e2 > -dy) {
					err -= dy;
					x1 += sx;
				}
				
				if(e2 < dx) {
					err += dx;
					y1 += sy;
				}
			}
		}

		public static void DrawRect(this RenderContext ctx, int x, int y, int w, int h, Cell value) {
			ctx.DrawLine( x, y, x + w, y, value);
			ctx.DrawLine(x + w, y, x + w, y + h, value);
			ctx.DrawLine(x + w, y + h, x, y + h, value);
			ctx.DrawLine(x, y + h, x, y, value);
		}

		public static void DrawBoundingBox(this RenderContext ctx, SpatialNode node, BoundingBox2D<int> box) {
			ctx.DrawRect(
				node.GlobalPosition.X + box.Min.X, node.GlobalPosition.Y + box.Min.Y,
				box.Max.X, box.Max.Y,
				BOUNDING_BOX_LINE
			);
		}
	}
}