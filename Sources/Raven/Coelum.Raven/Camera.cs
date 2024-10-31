using Coelum.Raven.Display;
using Coelum.Raven.Node;
using Coelum.Raven.Shader;
using Coelum.Raven.Shader.Cell;
using Silk.NET.Maths;

namespace Coelum.Raven {
	
	public class Camera : SpatialNode, IShaderActivator {

		private readonly IDisplay _display;

	#region Shaders
		public List<ICellShader> CellShaders { get; } = new();
		public List<ICellShader> SpatialShaders { get; } = new();
	#endregion

		/// <summary>
		/// vX | vW (u) | cX | cW
		/// vY | vH (u) | cY | cH
		/// </summary>
		public Matrix2X4<int> ViewMatrix;

		public int ViewX {
			get => ViewMatrix.M11;
			set => ViewMatrix.M11 = value;
		}

		public int ViewY {
			get => ViewMatrix.M21;
			set => ViewMatrix.M21 = value;
		}

		public int ViewWidth {
			get => ViewMatrix.M12;
			set => ViewMatrix.M12 = value;
		}

		public int ViewHeight {
			get => ViewMatrix.M22;
			set => ViewMatrix.M22 = value;
		}

		public int ClipX {
			get => ViewMatrix.M13;
			set => ViewMatrix.M13 = value;
		}
		
		public int ClipY {
			get => ViewMatrix.M23;
			set => ViewMatrix.M23 = value;
		}
		
		public int ClipWidth {
			get => ViewMatrix.M14;
			set => ViewMatrix.M14 = value;
		}
		
		public int ClipHeight {
			get => ViewMatrix.M24;
			set => ViewMatrix.M24 = value;
		}
		
		public Camera(RenderContext ctx) {
			_display = ctx.Display;

			ViewMatrix = new(
				0, _display.Width,
				0, _display.Width,
				0, _display.Height,
				0, _display.Height
			);
			
			CellShaders.Add(new CameraShader(this));
			
			RecalculateViewMatrix();
			ctx.Display.Resize += (_, _) => RecalculateViewMatrix();

			Added += _ => ((IShaderActivator) this).ActivateShaders(ctx);
			Removed += _ => ((IShaderActivator) this).DeactivateShaders(ctx);
		}

		public void RecalculateViewMatrix() {
			int widthRatio = Math.Max(_display.Width, ViewWidth) / Math.Min(_display.Width, ViewWidth);
			int heightRatio = Math.Max(_display.Height, ViewHeight) / Math.Min(_display.Height, ViewHeight);
			
			// TODO is this correct?
			ViewMatrix = new(
				-GlobalPosition.X, _display.Width,
				widthRatio * ClipX, widthRatio * ClipWidth,
				-GlobalPosition.Y, _display.Height,
				heightRatio * ClipY, heightRatio * ClipHeight
			);
		}
	}
}