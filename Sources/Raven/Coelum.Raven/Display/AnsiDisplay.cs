using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Coelum.LanguageExtensions;
using Coelum.Raven.Debug;
using Coelum.Raven.Shader;

namespace Coelum.Raven.Display {
	
	public class AnsiDisplay : IDisplay {

		public const string ESC = "\x1B";
		public const string CSI = "\x1B[";
		public const string DCS = "\x1BP";
		public const string OSC = "\x1B]";
		
		public int Width { get; }
		public int Height { get; }

		public TextWriter Out { get; }

		public AnsiDisplay(int width, int height, TextWriter _out) {
			Width = width;
			Height = height;
			Out = _out;
		}

		public void HideCursor() {
			Out.Write($"{CSI}?25l");
		}
		public void ShowCursor() {
			Out.Write($"{CSI}?25h");
		}

		public void Clear() {
			Out.Write($"{CSI}2J");
		}

		public void SwapBuffers(ref Cell[,] backBuffer, ref Cell[,] frontBuffer) {
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void WriteCell(Cell cell) {
				Out.WriteMultiple(
					CSI, "38;2;",
					cell.ForegroundColor.R, ";", cell.ForegroundColor.G, ";", cell.ForegroundColor.B,
					"m"
				);

				Out.WriteMultiple(
					CSI, "48;2;",
					cell.BackgroundColor.R, ";", cell.BackgroundColor.G, ";", cell.BackgroundColor.B,
					"m"
				);
						
				Out.Write(cell.Character);
			}
			
			for(int y = 0; y < Height; y++) {
				for(int x = 0; x < Width; x++) {
					Console.SetCursorPosition(x, y);
					
					if(!backBuffer[y, x].Equals(frontBuffer[y, x])) {
						Cell cell;

						if(RenderingFlags.VisualizeDirtyCells) {
							cell = new() {
								Character = 'D',
								ForegroundColor = Color.FromArgb(192, 192, 192),
								BackgroundColor = Color.White
							};
						} else {
							cell = backBuffer[y, x];
						}

						WriteCell(cell);
						
						frontBuffer[y, x] = backBuffer[y, x];
					} else if(RenderingFlags.VisualizeDirtyCells) {
						WriteCell(new() {
							Character = 'C',
							ForegroundColor = Color.FromArgb(64, 64, 64),
							BackgroundColor = Color.Black
						});
					}
				}
			}
		}
	}
}