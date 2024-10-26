namespace Coelum.Raven.Display {
	
	public interface IDisplay {
		
		public int Width { get; }
		public int Height { get; }
		
		public TextWriter Out { get; }

		public void HideCursor();
		public void ShowCursor();
		
		public void Clear();
		public void SwapBuffers(ref Cell[,] backBuffer, ref Cell[,] frontBuffer);
	}
}