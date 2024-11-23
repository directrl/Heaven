namespace Coelum.Resources {
	
	public sealed class ResourceType {

		public static readonly ResourceType CUSTOM = new("", "");
		public static readonly ResourceType DIRECTORY = new("", "");
		public static readonly ResourceType SHADER = new("Shaders", "");
		public static readonly ResourceType TEXTURE = new("Textures", "");
		public static readonly ResourceType MODEL = new("Models", "");
			
		public string Path { get; }
		public string Extension { get; }

		public ResourceType(string path, string extension) {
			Path = path;
			Extension = extension;
		}

		public static ResourceType MatchPath(string fullPath) {
			fullPath = fullPath.Replace('/', '.');
			string[] components = fullPath.Split('.');

			foreach(var component in components) {
				switch(component) {
					case "Shaders":
						return SHADER;
					case "Textures":
						return TEXTURE;
					case "Models":
						return MODEL;
				}
			}

			return CUSTOM;
		}
	}
}