using System.Reflection;
using System.Text;
using Serilog;

namespace Coeli.Resources {
	
	public struct Resource : IEquatable<Resource> {

		private readonly Assembly _assembly;
		private readonly Type _type;
		private readonly string _namespace;
		private readonly string _name;

		public string FullPath { get; init; }
		public string UID => FullPath;

		public bool Cache { get; set; } = true;

		internal Resource(Type type, string _namespace, string name, Assembly? assembly = null) {
			if(assembly == null) {
				_assembly = Assembly.GetCallingAssembly();
			} else {
				_assembly = assembly;
			}
			
			_type = type;
			this._namespace = _namespace;
			_name = name;

			FullPath = _namespace + "/" + type.Path + "/" + name + type.Extension;
		}

		public Stream? GetStream() {
			if(Cache && ResourceCache.GLOBAL.TryGet(this, out var data)) {
				return new MemoryStream(data);
			}
			
			Stream? stream;
			
			try {
				stream = _assembly
					.GetManifestResourceStream($"{_namespace}.{_type.Path}.{_name}{_type.Extension}");
			} catch(Exception e) {
				Log.Logger.Error($"Failed to get stream for resource [{this}]", e);
				return null;
			}

			if(stream == null) Log.Logger.Error($"Failed to get stream for resource [{this}]");
			return stream;
		}

		public byte[]? ReadBytes() {
			if(Cache && ResourceCache.GLOBAL.TryGet(this, out var data)) {
				return data;
			}
			
			using(var stream = GetStream()) {
				if(stream == null) return null;
				
				using(var memoryStream = new MemoryStream()) {
					stream.CopyTo(memoryStream);
					data = memoryStream.ToArray();
					
					if(Cache) ResourceCache.GLOBAL.Set(this, data);
					return data;
				}
			}
		}

		public string? ReadString(Encoding? encoding = null) {
			encoding ??= Encoding.UTF8;

			var data = ReadBytes();
			if(data != null) return encoding.GetString(data);
			return null;
		}

		public override string ToString() {
			return UID;
		}

		public bool Equals(Resource other) {
			return UID == other.UID;
		}

		public static bool operator==(Resource res1, Resource res2) {
			return res1.Equals(res2);
		}
		
		public static bool operator!=(Resource res1, Resource res2) {
			return !res1.Equals(res2);
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