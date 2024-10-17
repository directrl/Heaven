using System.Collections.Concurrent;
using Coeli.LanguageExtensions;

namespace Coeli.Resources {
	
	public class ResourceCache : CacheBase<Resource, byte[]> {

		public static readonly ResourceCache GLOBAL = new();
	}
}