namespace Coeli.Resources {
	
	public class ExternalResourceManager : ResourceManager {
	
		public ExternalResourceManager() : base(null) { }

		public override Resource Get(Resource.Type type, string name)
			=> throw new NotImplementedException();
		
		public override Resource this[Resource.Type type, string name]
			=> throw new NotImplementedException();

		public ExternalResource Get(string path) => new ExternalResource(path);
		public ExternalResource this[string path] => new ExternalResource(path);
	}
}