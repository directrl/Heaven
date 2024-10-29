using Flecs.NET.Core;

namespace Coelum.Common.Ecs {
	
	public class PrefabManager {

		private readonly Dictionary<string, IPrefab> _prefabs = new();
		private readonly World _world;

		public PrefabManager(World world) {
			_world = world;
		}

		public void Add<T>(string? name = null) where T : IPrefab, new() {
			name ??= typeof(T).Name;
			var p = new T();
			_prefabs[name] = p;
			p.Setup(_world);
		}

		public T Get<T>(string? name = null) where T : IPrefab {
			name ??= typeof(T).Name;
			return (T) _prefabs[name];
		}

		public Entity Create<T>(string? name = null) where T : IPrefab {
			return Get<T>(name).Create(_world);
		}
		
		public IPrefab this[string name] {
			get => _prefabs[name];
			set => _prefabs[name] = value;
		}
	}
}