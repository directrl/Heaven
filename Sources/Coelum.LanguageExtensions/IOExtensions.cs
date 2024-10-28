namespace Coelum.LanguageExtensions {
	
	public static class IOExtensions {

		public static void WriteMultiple(this TextWriter writer, params object[] values) {
			foreach(object value in values) {
				writer.Write(value);
			}
		}
	}
}