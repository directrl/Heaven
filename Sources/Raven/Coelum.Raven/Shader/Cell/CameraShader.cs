namespace Coelum.Raven.Shader.Cell {
	
	public class CameraShader : ICellShader {
		
		public Camera Camera { get; }

		public CameraShader(Camera camera) {
			Camera = camera;
		}

		public bool Process(ref ICellShader.Parameter input) {
			input.Position.X += Camera.ViewX;
			input.Position.Y += Camera.ViewY;

			int x = input.Position.X;
			int y = input.Position.Y;

			if(x < Camera.ClipX || x > (Camera.ClipX + Camera.ClipWidth)
			   || y < Camera.ClipY || y > (Camera.ClipY + Camera.ClipHeight)) return false; // discard

			return true;
		}
	}
}