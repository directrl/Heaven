using Coelum.Graphics;
using Coelum.LanguageExtensions;
using Coelum.Resources;

namespace Coelum.ModelLoading {

	class ModelCache : CacheBase<IResource, Model> {

		public static readonly ModelCache GLOBAL = new();
	}
}