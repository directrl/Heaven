using Coelum.Phoenix;
using Coelum.LanguageExtensions;
using Coelum.Resources;

namespace Coelum.Phoenix.ModelLoading {

	class ModelCache : CacheBase<IResource, Model> {

		public static readonly ModelCache GLOBAL = new();
	}
}