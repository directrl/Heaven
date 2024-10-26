using System.Drawing;
using Coelum.Debug;
using Coelum.Raven.Display;
using Coelum.Raven.Shader;
using Silk.NET.Maths;

namespace Coelum.Raven {
	
	public class RenderContext {

		public static readonly Cell DEFAULT_CELL = new();
		
		public IDisplay	Display { get; }

		public Cell[,] BackBuffer;
		public Cell[,] FrontBuffer;

	#region Shaders
		public List<IFragmentShader> FragmentShaders { get; } = new();
	#endregion

		public RenderContext(IDisplay display) {
			Display = display;
			
			BackBuffer = new Cell[display.Height, display.Width];
			FrontBuffer = new Cell[display.Height, display.Width];

			Clear(ref BackBuffer);
		}

		public void Clear(ref Cell[,] buffer, Cell? value = null) {
			for(int y = 0; y < Display.Height; y++) {
				for(int x = 0; x < Display.Width; x++) {
					buffer[y, x] = value ?? DEFAULT_CELL;
				}
			}
		}

		public void Render() {
			Display.SwapBuffers(ref BackBuffer, ref FrontBuffer);
		}

		public void Reset() {
			FragmentShaders.Clear();
			Clear(ref BackBuffer);
			Render();
		}
		
		public Cell? this[int x, int y] {
			get => this[new(x, y)];
			set => this[new(x, y)] = value;
		}

		public Cell? this[Vector2D<int> pos] {
			get {
				if(pos.X < 0 || pos.X >= BackBuffer.GetLength(1)
				   || pos.Y < 0 || pos.Y >= BackBuffer.GetLength(0)) {

					return null;
				}

				return BackBuffer[pos.Y, pos.X];
			}
			set {
				Tests.Assert(value.HasValue);
				
				var frag = new IFragmentShader.Parameter() {
					Position = pos,
					Cell = value.Value
				};

				foreach(var fragShader in FragmentShaders) {
					fragShader.Process(ref frag);
				}
				
				if(frag.Position.X < 0 || frag.Position.X >= Display.Width
				   || frag.Position.Y < 0 || frag.Position.Y >= Display.Height) {
					
					return;
				}

				BackBuffer[frag.Position.Y, frag.Position.X] = frag.Cell;
			}
		}
	}
}