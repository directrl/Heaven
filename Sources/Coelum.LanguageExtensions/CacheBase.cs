using System.Collections.Concurrent;
using System.Reflection;
using Serilog;

namespace Coelum.LanguageExtensions {
	
	public class CacheBase<TKey, TValue> {
		
		private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _cache = new();

		public bool Has(TKey key) {
			return _cache.ContainsKey(key);
		}

		public bool TryGet(TKey key, out TValue value) {
			if(_cache.TryGetValue(key, out var lazy)) {
				value = lazy.Value;
				return true;
			}

			value = default;
			return false;
		}

		public void Set(TKey key, TValue value) {
			Log.Verbose($"CacheBase: Caching [{key}] to [{value}]");
			
			_cache.AddOrUpdate(key,
				_ => new(() => value),
				(_, _) => new(() => value));
		}
	}
}