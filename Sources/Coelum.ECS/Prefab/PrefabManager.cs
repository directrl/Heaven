using System.Reflection;
using Coelum.Debug;
using Serilog;

namespace Coelum.ECS.Prefab {
	
	public class PrefabManager {

		public Dictionary<string, IPrefab> Prefabs { get; } = new();
		public NodeRoot Root { get; }

		public PrefabManager(NodeRoot root, Assembly? assembly) {
			Root = root;
			
			if(assembly is not null) AddAssembly(assembly);
		}

		public void AddAssembly(Assembly assembly, bool strictType = true) {
			var prefabTypes = PrefabScanner.ScanAssembly(assembly, strictType);

			foreach(var prefabType in prefabTypes) {
				var attribute = prefabType.GetCustomAttribute<PrefabAttribute>();

				if(attribute is null) {
					Log.Logger.Warning($"[PREFAB MANAGER] Prefab [{prefabType.Name}] does not have PrefabAttribute, skipping");
					continue;
				}

				var ctor = prefabType.GetConstructor(Type.EmptyTypes);
				
				if(ctor is null) {
					Log.Logger.Warning($"[PREFAB MANAGER] Prefab [{prefabType.Name}] does not have a default constructor, skipping");
					continue;
				}

				var prefab = (IPrefab) ctor.Invoke(null);
				Prefabs.Add(attribute.Name, prefab);
			}
		}

		public Node Create<T>() where T : IPrefab {
			var attribute = typeof(T).GetCustomAttribute<PrefabAttribute>();
			Tests.Assert(attribute != null, "T does not have PrefabAttribute");

			var ctor = typeof(T).GetConstructor(Type.EmptyTypes);
			Tests.Assert(ctor != null, "T does not have a default constructor");

			var prefab = (IPrefab) ctor.Invoke(null);
			return prefab.Create(Root);
		}

		public Node Create(string name) {
			return Prefabs[name].Create(Root);
		}
	}
}