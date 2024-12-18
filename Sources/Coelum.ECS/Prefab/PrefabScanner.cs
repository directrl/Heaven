using System.Reflection;

namespace Coelum.ECS.Prefab {
	
	public static class PrefabScanner {
		
		public static List<Type> ScanAssembly(Assembly assembly, bool strictType = true) {
			var nodes = new List<Type>();

			foreach(var type in assembly.GetExportedTypes()) {
				if(type != typeof(Node) && typeof(Node).IsAssignableFrom(type)
				   && (!strictType || (type != typeof(IPrefab) && typeof(IPrefab).IsAssignableFrom(type)))) {
					nodes.Add(type);
				}
			}
			
			return nodes;
		}
	}
}