using Silk.NET.Maths;

namespace Coelum.Raven.Shader {
	
	public interface IFragmentShader : IShader<IFragmentShader.Parameter> {
		
		public RenderContext Context { get; }

		public struct Parameter {

			public Vector2D<int> Position;
			public Cell Cell;
		}
	}
}