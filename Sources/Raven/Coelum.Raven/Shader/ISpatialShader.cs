using Coelum.Raven.Node;
using Silk.NET.Maths;

namespace Coelum.Raven.Shader {
	
	public interface ISpatialShader : IShader<ISpatialShader.Parameter> {
		
		public struct Parameter {
			
			public Vector2D<int> Position;
			public Raven.Cell Cell;
			public SpatialNode SpatialNode;
		}
	}
}