namespace Coelum.LanguageExtensions {
	
	public static class TypeExtensions {

		public static Type GetDeepBaseType(this Type type, Type? limit = null) {
			limit ??= typeof(object);

			if(type == limit) return type;

			Type prevType = type;
			Type baseType = type;
			
			while((baseType = baseType.BaseType) != limit) {
				prevType = baseType;
			}

			return prevType;
		}
	}
}