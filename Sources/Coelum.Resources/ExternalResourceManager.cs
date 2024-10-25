namespace Coelum.Resources {
	
	public class ExternalResourceManager : ResourceManager {
	
		public ExternalResourceManager() : base("") { }

		public override IResource Get(ResourceType type, string path)
			=> new ExternalResource(type, path);
		
		public override IResource this[ResourceType type, string path]
			=> new ExternalResource(type, path);
	}
}