namespace Coelum.Raven.Shader.Spatial {
	
	public class PositionShader : ISpatialShader {

		public bool Process(ref ISpatialShader.Parameter input) {
			input.Position = input.SpatialNode.GlobalPosition;
			
			return true;
		}
	}
}