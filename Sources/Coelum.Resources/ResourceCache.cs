using System.Collections.Concurrent;
using Coelum.LanguageExtensions;

namespace Coelum.Resources {
	
	public class ResourceCache : CacheBase<Resource, byte[]> {

		public static readonly ResourceCache GLOBAL = new();
	}
}