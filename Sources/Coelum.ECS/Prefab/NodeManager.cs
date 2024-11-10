using System.Reflection;
using Coelum.Debug;
using Serilog;

namespace Coelum.ECS.Prefab {
	
	public class NodeManager {

		public Dictionary<string, Node> Nodes { get; } = new();
		public NodeRoot Root { get; }

		public NodeManager(NodeRoot root, Assembly? assembly) {
			Root = root;
			
			if(assembly is not null) AddAssembly(assembly);
		}

		public void AddAssembly(Assembly assembly, bool strictType = true) {
			var nodeTypes = PrefabScanner.ScanAssembly(assembly, strictType: false);

			foreach(var nodeType in nodeTypes) {
				var ctor = nodeType.GetConstructor(Type.EmptyTypes);
				
				if(ctor is null) {
					Log.Logger.Warning($"[NODE MANAGER] Node [{nodeType.Name}] does not have a default constructor, skipping");
					continue;
				}

				var node = (Node) ctor.Invoke(null);
				Nodes.Add(nodeType.Name, node);
			}
		}

		public Node Create<T>() where T : Node {
			var ctor = typeof(T).GetConstructor(Type.EmptyTypes);
			Tests.Assert(ctor != null, "T does not have a default constructor");

			var node = (Node) ctor.Invoke(null);
			return node;
		}

		public Node Create(string name) {
			var t = Nodes[name].GetType();
			
			var ctor = t.GetConstructor(Type.EmptyTypes);
			Tests.Assert(ctor != null, "T does not have a default constructor");

			var node = (Node) ctor.Invoke(null);
			return node;
		}
	}
}