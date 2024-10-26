namespace Coelum.Raven.Shader.Fragment {
	
	public class CameraViewShader : IFragmentShader {
		
		public RenderContext Context { get; }
		public Camera Camera { get; }

		public CameraViewShader(RenderContext ctx, Camera camera) {
			Context = ctx;
			Camera = camera;
		}

		public void Process(ref IFragmentShader.Parameter input) {
			// TODO
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