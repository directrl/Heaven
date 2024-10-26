namespace Coelum.Raven {
	
	public static class Primitives {
		
		/*public static void DrawLine(RenderContext ctx, int x1, int y1, int x2, int y2, Cell value) {
			var pixels = new List<(int x, int y)>();

			int dx = Math.Abs(x2 - x1);
			int dy = Math.Abs(y2 - y1);
			int sx = x1 < x2 ? 1 : -1;
			int sy = y1 < y2 ? 1 : -1;
			int err = dx - dy;

			while(true) {
				pixels.Add((x1, y1));

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

				return pixels;

			var p = GetLinePixels(x1, y1, x2, y2);
			foreach(var a in p) {
				var x = a.x;
				var y = a.y;
				if(x >= 0 && x < Display.Width && y >= 0 && y < Display.Height) {
					BackBuffer[y, x] = value;
				}
			}
		}

		public void DrawRect(int x, int y, int w, int h, Cell value) {
			DrawLine(x, y, x + w, y, value);
			DrawLine(x + w, y, x + w, y + h, value);
			DrawLine(x + w, y + h, x, y + h, value);
			DrawLine(x, y + h, x, y, value);
		}*/
	}
}