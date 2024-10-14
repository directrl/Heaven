using System.Reflection;

namespace Coeli.Resources {
	
	public class ResourceManager {

		private readonly Assembly _assembly;
		
		public string Namespace { get; }
		
		public ResourceManager(string _namespace, Assembly? assembly = null) {
			if(assembly == null) assembly = Assembly.GetCallingAssembly();
			
			_assembly = assembly;
			Namespace = _namespace;
		}

		public virtual Resource Get(Resource.Type type, string name)
			=> new(type, Namespace, name, _assembly);

		public virtual Resource this[Resource.Type type, string name]
			=> new(type, Namespace, name, _assembly);
	}
}