using Silk.NET.Maths;

namespace Coelum.Raven.Shader {
	
	public interface ICellShader : IShader<ICellShader.Parameter> {

		public struct Parameter {

			public Vector2D<int> Position;
			public Raven.Cell Cell;
		}
	}
}