using System.Drawing;
using System.Runtime.CompilerServices;
using Silk.NET.Maths;

namespace Coelum.Raven.Terminal {
	
	public abstract class TerminalBase {

		public static readonly Color DEFAULT_FOREGROUND = Color.White;
		public static readonly Color DEFAULT_BACKGROUND = Color.Black;
		
		public abstract ICursor Cursor { get; }
		
		public abstract int Width { get; set; }
		public abstract int Height { get; set; }

		protected TerminalEntry[,] Buffer;

		protected TerminalBase(int width, int height) {
			Buffer = new TerminalEntry[width, height];

			for(int y = 0; y < height; y++) {
				for(int x = 0; x < width; x++) {
					Buffer[x, y] = new() {
						Foreground = DEFAULT_FOREGROUND,
						Background = DEFAULT_BACKGROUND,
						Character = ' '
					};
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(TerminalEntry value, Vector2D<int>? position = null) {
			Buffer[position?.X ?? 0, position?.Y ?? 0] = value;
		}

		public void Write(char value,
		                  Vector2D<int>? position = null,
		                  Color? foreground = null,
		                  Color? background = null) {
			
			foreground ??= DEFAULT_FOREGROUND;
			background ??= DEFAULT_BACKGROUND;

			int x = position?.X ?? Cursor.X;
			int y = position?.Y ?? Cursor.Y;

			Buffer[x, y].Foreground = foreground.Value;
			Buffer[x, y].Background = background.Value;
			Buffer[x, y].Character = value;
		}

		public void Write(string value,
		                  Vector2D<int>? position = null,
		                  Color? foreground = null,
		                  Color? background = null) {

			foreground ??= DEFAULT_FOREGROUND;
			background ??= DEFAULT_BACKGROUND;

			int y = position?.Y ?? Cursor.Y;
			int i = 0;
			
			for(int x = (position?.X ?? Cursor.X); x < (position?.X ?? Cursor.X) + value.Length; x++) {
				if(x > Buffer.GetLength(0)) break;
				
				Buffer[x, y].Foreground = foreground.Value;
				Buffer[x, y].Background = background.Value;
				Buffer[x, y].Character = value[i++];
			}
		}
		
		public void Clear(char clear = ' ', Vector2D<int>? start = null, Vector2D<int>? end = null) {
			if(start == null && end == null) Clear();
			
			for(int y = (start?.Y ?? 0); y < (end?.Y ?? Height); y++) {
				for(int x = (start?.X ?? 0); x < (end?.X ?? Width); x++) {
					Buffer[x, y].Character = clear;
				}
			}
		}

		protected abstract void Clear();
		public abstract void Refresh();
	}
	
	public struct TerminalEntry {
		
		public Color Foreground { get; set; }
		public Color Background { get; set; }
		public char Character { get; set; }
	}
	
	public interface ICursor {
			
		public int X { get; set; }
		public int Y { get; set; }
			
		public bool Visible { get; set; }
	}
}