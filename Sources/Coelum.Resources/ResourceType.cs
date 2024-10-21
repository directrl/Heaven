namespace Coelum.Resources {
	
	public sealed class ResourceType {

		public static readonly ResourceType CUSTOM = new("", "");
		public static readonly ResourceType SHADER = new("Shaders", "");
		public static readonly ResourceType TEXTURE = new("Textures", ".png");
		public static readonly ResourceType MODEL = new("Models", "");
			
		public string Path { get; }
		public string Extension { get; }

		private ResourceType(string path, string extension) {
			Path = path;
			Extension = extension;
		}
	}
}