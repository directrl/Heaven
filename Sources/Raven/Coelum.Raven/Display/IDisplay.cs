namespace Coelum.Raven.Display {
	
	public interface IDisplay {

	#region Delegates
		public delegate void ResizeEventHandler(int width, int height);
	#endregion

	#region Events
		public event ResizeEventHandler? Resize;
	#endregion
		
		public int Width { get; set; }
		public int Height { get; set; }
		
		public StreamWriter Out { get; }

		public void HideCursor();
		public void ShowCursor();
		
		public void Clear();
		public void SwapBuffers(ref Cell[,] backBuffer, ref Cell[,] frontBuffer);
	}
}