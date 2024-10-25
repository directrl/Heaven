using System.Text;

namespace Coelum.Raven.Terminal {
	
	public class AnsiTerminal : TerminalBase {

		public override ICursor Cursor { get; } = new AnsiCursor();

		public override int Width {
			get => Console.BufferWidth;
			set => throw new NotSupportedException();
		}
		
		public override int Height {
			get => Console.BufferHeight;
			set => throw new NotSupportedException();
		}
		
		public AnsiTerminal() : base(Console.BufferWidth, Console.BufferHeight) { }

		protected override void Clear() {
			Console.Clear();
		}

		public override void Refresh() {
			var builder = new StringBuilder(Buffer.Length);

			for(int y = 0; y < Buffer.GetLength(1); y++) {
				for(int x = 0; x < Buffer.GetLength(0); x++) {
					builder.Append(Buffer[x, y].Character);
				}

				//builder.Append('\n');
			}

			Clear();
			
			Cursor.X = 0;
			Cursor.Y = 0;
			
			Console.Write(builder.ToString());
		}
	}

	public class AnsiCursor : ICursor {

		public int X {
			get => Console.CursorLeft;
			set => Console.CursorLeft = value;
		}
		
		public int Y {
			get => Console.CursorTop;
			set => Console.CursorTop = value;
		}
		
		public bool Visible {
			get => throw new NotSupportedException();
			set => Console.CursorVisible = value;
		}
	}
}