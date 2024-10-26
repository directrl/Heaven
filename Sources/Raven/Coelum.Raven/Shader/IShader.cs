namespace Coelum.Raven.Shader {
	
	public interface IShader<TParam> {

		public void Process(ref TParam input);
	}
}