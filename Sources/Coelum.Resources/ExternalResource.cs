using Serilog;

namespace Coelum.Resources {
	
	public class ExternalResource {
		
		public string FullPath { get; }
		public string UID => FullPath;

		internal ExternalResource(string path) {
			FullPath = path;
		}

		public Stream? GetStream() {
			try {
				return new FileStream(FullPath, FileMode.Open, FileAccess.Read);
			} catch(Exception e) {
				Log.Logger.Warning($"Failed to get stream for external resource {this}", e);
				return null;
			}
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
	}
}