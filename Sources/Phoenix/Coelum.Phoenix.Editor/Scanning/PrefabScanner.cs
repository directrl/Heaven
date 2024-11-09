using System.Reflection;
using Coelum.ECS;

namespace Coelum.Phoenix.Editor.Scanning {
	
	public class PrefabScanner {

		public static List<Type> Scan(Assembly assembly) {
			var nodes = new List<Type>();

			foreach(var type in assembly.GetExportedTypes()) {
				if(type != typeof(Node) && typeof(Node).IsAssignableFrom(type)) {
					nodes.Add(type);
				}
			}
			
			return nodes;
		}
	}
}