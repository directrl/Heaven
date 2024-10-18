namespace Coelum.LanguageExtensions {
	
	public interface ISingleton<T> where T : new() {

		protected static readonly T _instance = new T();
	}
	
	public interface ILazySingleton<T> where T : new() {

		protected static readonly Lazy<T> _instance = new(() => new T());
	}
}