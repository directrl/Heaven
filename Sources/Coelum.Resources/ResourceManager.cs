using System.Reflection;

namespace Coelum.Resources {
	
	public class ResourceManager {

		private readonly Assembly _assembly;
		
		public string Namespace { get; }
		
		public ResourceManager(string _namespace, Assembly? assembly = null) {
			if(assembly == null) assembly = Assembly.GetCallingAssembly();
			
			_assembly = assembly;
			Namespace = _namespace;
		}

		public virtual IResource Get(ResourceType type, string name)
			=> new Resource(type, Namespace, name, _assembly);

		public virtual IResource this[ResourceType type, string name]
			=> new Resource(type, Namespace, name, _assembly);
	}
}