using System.Text;
using Serilog;

namespace Coelum.Resources {
	
	public struct ExternalResource : IResource {
		
		public string Path { get; }

		public string Name { get; }
		public ResourceType Type { get; }
		
		public string UID { get; }

		public bool Cache { get; set; } = false;

		public ExternalResource(ResourceType type, string path) {
			Type = type;
			Name = path;
			Path = path;
			
			UID = $"EXT({type.Path}):" + path;
		}

		public Stream? GetStream() {
			try {
				return new FileStream(Path, FileMode.Open, FileAccess.Read);
			} catch(Exception e) {
				Log.Logger.Warning($"Failed to get stream for external resource {this}", e);
				return null;
			}
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

		public static bool operator==(ExternalResource res1, IResource res2) {
			return res1.UID == res2.UID;
		}
		
		public static bool operator!=(ExternalResource res1, IResource res2) {
			return res1.UID != res2.UID;
		}
	}
}