namespace Coelum.Raven.Shader {
	
	public interface IShader<TParam> {

		public bool Process(ref TParam input);
	}
}