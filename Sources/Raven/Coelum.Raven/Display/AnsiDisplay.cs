using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Coelum.LanguageExtensions;
using Coelum.Raven.Debug;
using Coelum.Raven.Shader;

namespace Coelum.Raven.Display {
	
	public class AnsiDisplay : IDisplay {

	#region Events
		public event IDisplay.ResizeEventHandler? Resize;
	#endregion

		public const string ESC = "\x1B";
		public const string CSI = "\x1B[";
		public const string DCS = "\x1BP";
		public const string OSC = "\x1B]";

		public int Width { get; set; }
		public int Height { get; set; }

		public StreamWriter Out { get; }

		public AnsiDisplay(StreamWriter _out) {
			Width = Console.BufferWidth;
			Height = Console.BufferHeight;
			Out = _out;
			
			Out.AutoFlush = false;
		}

		public void HideCursor() {
			Out.Write($"{CSI}?25l");
		}
		
		public void ShowCursor() {
			Out.Write($"{CSI}?25h");
		}

		public void Clear() {
			Out.Write($"{CSI}2J");
			Out.Write($"{CSI}0;0H");
		}

		public void SwapBuffers(ref Cell[,] backBuffer, ref Cell[,] frontBuffer) {
			if(Console.BufferWidth != Width || Console.BufferHeight != Height) {
				Width = Console.BufferWidth;
				Height = Console.BufferHeight;
				
				Resize?.Invoke(Width, Height);
			}
			
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
			
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			void WriteCellPos(int x, int y, Cell cell) {
				Out.WriteMultiple(
					CSI,
					y + 1, ";", x + 1,
					"H"
				);
				
				WriteCell(cell);
			}

			if(RenderingFlags.UseDirtyRendering) {
				for(int y = 0; y < Height; y++) {
					for(int x = 0; x < Width; x++) {
						if(!backBuffer[y, x].Equals(frontBuffer[y, x])) {
							var cell = backBuffer[y, x];

							WriteCellPos(x, y, cell);
							frontBuffer[y, x] = cell;
						}
					}
				}
			} else {
				Out.WriteMultiple(CSI, "0;0H");
				
				foreach(var cell in backBuffer) {
					WriteCell(cell);
				}
			}
			
			Out.Flush();
		}
	}
}