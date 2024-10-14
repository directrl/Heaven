using System.Reflection;
using Serilog;

namespace Coeli.Resources {
	
	public readonly struct Resource {

		private readonly Assembly _assembly;
		private readonly Type _type;
		private readonly string _namespace;
		private readonly string _name;

		public string FullPath { get; }
		public string UID => FullPath;

		internal Resource(Type type, string _namespace, string name, Assembly? assembly = null) {
			if(assembly == null) {
				_assembly = Assembly.GetCallingAssembly();
			} else {
				_assembly = assembly;
			}
			
			_type = type;
			this._namespace = _namespace;
			_name = name;

			FullPath = _namespace + "/" + type.Path + "/" + name;
		}

		public Stream? GetStream() {
			Stream? stream;
			
			try {
				stream = _assembly
					.GetManifestResourceStream($"{_namespace}.{_type.Path}.{_name}{_type.Extension}");
			} catch(Exception e) {
				Log.Logger.Warning($"Failed to get stream for resource {this}", e);
				return null;
			}

			if(stream == null) Log.Logger.Warning($"Failed to get stream for resource {this}");
			return stream;
		}

		public byte[]? ReadBytes() {
			using(var stream = GetStream()) {
				if(stream == null) return null;
				
				using(var memoryStream = new MemoryStream()) {
					stream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
		}

		public string? ReadString() {
			using(var stream = GetStream()) {
				if(stream == null) return null;
				
				using(var reader = new StreamReader(stream)) {
					return reader.ReadToEnd();
				}
			}
		}

		public override string ToString() {
			return UID;
		}

		public sealed class Type {
		
			public static readonly Type SHADER = new("Shaders", "");
			public static readonly Type TEXTURE = new("Textures", ".png");
			public static readonly Type MODEL = new("Models", "");
			
			public string Path { get; }
			public string Extension { get; }

			private Type(string path, string extension) {
				Path = path;
				Extension = extension;
			}
		}
	}
}