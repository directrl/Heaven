using Coelum.ECS;
using Coelum.Phoenix.Camera;
using Silk.NET.Maths;

namespace Coelum.Phoenix {
	
	public class Viewport : Node {
		
		public CameraBase Camera { get; set; }

		private Framebuffer _framebuffer;
		public Framebuffer Framebuffer {
			get => _framebuffer;
			set {
				_framebuffer.Resize -= _FramebufferResizeHandler;
				_framebuffer = value;
				value.Resize += _FramebufferResizeHandler;
			}
		}

		public bool Enabled { get; set; } = true;

		public Viewport(CameraBase camera, Framebuffer framebuffer) {
			Camera = camera;
			_framebuffer = framebuffer;

			_FramebufferResizeHandler(framebuffer.Size); // initial resize
		}

	#region Event handlers
		private void _FramebufferResizeHandler(Vector2D<int> newSize) {
			Camera.Width = newSize.X;
			Camera.Height = newSize.Y;
			Camera.RecalculateProjectionMatrix();
		}
	#endregion
	}
}