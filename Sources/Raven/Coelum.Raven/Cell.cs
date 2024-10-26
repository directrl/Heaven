using System.Drawing;

namespace Coelum.Raven {
	
	public struct Cell : IEquatable<Cell> {

		public char Character { get; set; }
		public Color ForegroundColor { get; set; }
		public Color BackgroundColor { get; set; }

		public Cell() {
			Character = ' ';
			ForegroundColor = Color.White;
			BackgroundColor = Color.FromArgb(0, 0, 0, 0);
		}

		public bool Equals(Cell other) {
			return Equals((Cell?) other);
		}

		public bool Equals(Cell? other) {
			return Character == other?.Character
				&& ForegroundColor == other?.ForegroundColor
				&& BackgroundColor == other?.BackgroundColor;
		}

		public override bool Equals(object obj) {
			return obj is Cell && Equals((Cell) obj);
		}
	}
}