using System.Reflection;
using System.Text;
using Serilog;

namespace Coelum.Resources {
	
	public struct Resource : IResource {
		
		public Assembly Assembly { get; }
		public string Namespace { get; }
		
		public string Name { get; }
		public ResourceType Type { get; }

		public string UID { get; }

		public bool Cache { get; set; } = false;

		internal Resource(ResourceType type, string _namespace, string name, Assembly? assembly = null) {
			if(assembly == null) {
				Assembly = Assembly.GetCallingAssembly();
			} else {
				Assembly = assembly;
			}
			
			Type = type;
			Namespace = _namespace;
			Name = name;

			UID = _namespace + "/" + type.Path + "/" + name + type.Extension;
		}

		public Stream? GetStream() {
			if(Cache && ResourceCache.GLOBAL.TryGet(this, out var data)) {
				return new MemoryStream(data);
			}
			
			Stream? stream;
			
			try {
				stream = Assembly
					.GetManifestResourceStream($"{Namespace}.{Type.Path}.{Name.Replace('/', '.')}{Type.Extension}");
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

		public static bool operator==(Resource res1, IResource res2) {
			return res1.UID == res2.UID;
		}
		
		public static bool operator!=(Resource res1, IResource res2) {
			return res1.UID != res2.UID;
		}
	}
}