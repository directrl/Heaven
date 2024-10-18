namespace Coelum.Resources {
	
	public class ExternalResourceManager : ResourceManager {
	
		public ExternalResourceManager() : base(null) { }

		public override Resource Get(Resource.Type type, string name)
			=> throw new NotSupportedException();
		
		public override Resource this[Resource.Type type, string name]
			=> throw new NotSupportedException();

		public ExternalResource Get(string path) => new ExternalResource(path);
		public ExternalResource this[string path] => new ExternalResource(path);
	}
}