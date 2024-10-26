using Coelum.Node;
using Coelum.Raven.Display;
using Coelum.Raven.Shader;
using Coelum.Raven.Shader.Fragment;
using Silk.NET.Maths;

namespace Coelum.Raven {
	
	public class Camera : NodeBase {

		private readonly IDisplay _display;
		
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

		private FragmentShader _fragmentShader;
		
		public Camera(RenderContext ctx) {
			_display = ctx.Display;
			_fragmentShader = new(ctx, this);
			
			RecalculateViewMatrix();
			// TODO recalculate on resize

			Added += _ => {
				ctx.FragmentShaders.Add(_fragmentShader);
			};
			
			Removed += _ => {
				ctx.FragmentShaders.Remove(_fragmentShader);
			};
		}

		private void RecalculateViewMatrix() {
			ViewMatrix = new(
				0, _display.Width,
				0, _display.Width,
				0, _display.Width,
				0, _display.Height
			);
		}
		
		public class FragmentShader : IFragmentShader {
		
			public RenderContext Context { get; }
			public Camera Camera { get; }

			public FragmentShader(RenderContext ctx, Camera camera) {
				Context = ctx;
				Camera = camera;
			}

			public void Process(ref IFragmentShader.Parameter input) {
				// x += ViewMatrix.M11;
				// y += ViewMatrix.M21;
				// 	
				// int w = ViewMatrix.M12;
				// int h = ViewMatrix.M22;
				// 	
				// int clipX = ViewMatrix.M13;
				// int clipY = ViewMatrix.M23;
				//
				// int clipW = ViewMatrix.M14;
				// int clipH = ViewMatrix.M24;
				//
				// if(x < clipX || x > (clipX + clipW)
				//    || y < clipY || y > (clipY + clipH)) return;
				// 	
				// if(x >= 0 && x < w && y >= 0 && y < h) {
				// 	BackBuffer[y, x] = value ?? default;
				// }

				input.Position.X += Camera.ViewX;
				input.Position.Y += Camera.ViewY;
			}
		}
	}
}