using System.Drawing;
using Coelum.Debug;
using Coelum.Raven.Display;
using Coelum.Raven.Node;
using Coelum.Raven.Shader;
using Silk.NET.Maths;

namespace Coelum.Raven {
	
	public class RenderContext : IShaderActivator {

		public static readonly Cell DEFAULT_CELL = new();
		
		public IDisplay	Display { get; }

		public Cell[,] BackBuffer;
		public Cell[,] FrontBuffer;

	#region Shaders
		public List<ICellShader> CellShaders { get; } = new();
		public List<ICellShader> SpatialShaders { get; } = new();
	#endregion

		public RenderContext(IDisplay display) {
			Display = display;
			Display.Resize += (_, _) => Resize();
			
			// initial resize
			Resize();
		}

		public void Clear(ref Cell[,] buffer, Cell? value = null) {
			var cell = value ?? DEFAULT_CELL;
			
			for(int y = 0; y < Display.Height; y++) {
				for(int x = 0; x < Display.Width; x++) {
					buffer[y, x] = cell;
				}
			}
		}

		public void Render() {
			Display.SwapBuffers(ref BackBuffer, ref FrontBuffer);
		}

		public void Reset() {
			CellShaders.Clear();
			Clear(ref BackBuffer);
			Render();
		}

		public void Resize() {
			BackBuffer = new Cell[Display.Height, Display.Width];
			FrontBuffer = new Cell[Display.Height, Display.Width];
			
			Display.Clear();
			Clear(ref BackBuffer);
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
				if(!value.HasValue) return;

				var frag = new ICellShader.Parameter() {
					Position = pos,
					Cell = value.Value
				};

				foreach(var fragShader in CellShaders) {
					if(!fragShader.Process(ref frag)) return;
				}

				if(frag.Position.X < 0 || frag.Position.X >= Display.Width
				   || frag.Position.Y < 0 || frag.Position.Y >= Display.Height) return;

				BackBuffer[frag.Position.Y, frag.Position.X] = frag.Cell;
			}
		}
	}
}