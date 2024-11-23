using System.Reflection;
using System.Text;
using Serilog;

namespace Coelum.Resources {
	
	public struct Resource : IResource {
		
		public Assembly Assembly { get; }
		public string Namespace { get; }
		
		public string Name { get; }
		public ResourceType Type { get; }

		public string FullPath { get; }
		public string UID { get; }

		public bool Cache { get; set; } = false;

		internal Resource(ResourceType type, string @namespace, string name, Assembly? assembly = null) {
			if(assembly == null) {
				Assembly = Assembly.GetCallingAssembly();
			} else {
				Assembly = assembly;
			}
			
			Type = type;
			Namespace = @namespace;
			Name = name.Replace('/', '.');

			FullPath = @namespace;
			if(!string.IsNullOrWhiteSpace(type.Path)) FullPath += "." + type.Path;
			FullPath += "." + Name + type.Extension;
			
			UID = @namespace + "/" + type.Path + "/" + Name + type.Extension;
		}

		public Resource(ResourceType type, string path, Assembly? assembly = null) {
			if(assembly == null) {
				Assembly = Assembly.GetCallingAssembly();
			} else {
				Assembly = assembly;
			}

			Type = type;
			Namespace = Assembly.GetName().Name + ".Resources" ?? "";
			Name = path.Replace(Namespace + ".", "").Replace('/', '.');
			
			if(!string.IsNullOrWhiteSpace(type.Path)) {
				Name = Name.Replace(type.Path + ".", "");
			}
			
			if(!string.IsNullOrWhiteSpace(type.Extension)) {
				Name = Name.Replace(type.Extension, "");
			}
			
			FullPath = path;
			UID = Namespace + "/" + type.Path + "/" + Name + type.Extension;
		}

		public Stream? GetStream() {
			if(Cache && ResourceCache.GLOBAL.TryGet(this, out var data)) {
				return new MemoryStream(data);
			}
			
			Stream? stream;
			
			try {
				stream = Assembly
					.GetManifestResourceStream(FullPath.Replace('/', '.'));
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
		
		public string Export() {
			if(Type == ResourceType.DIRECTORY) {
				string tempDirPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				
				var resPaths = Assembly.GetManifestResourceNames();

				foreach(var path in resPaths) {
					if(!path.StartsWith(FullPath.Replace('/', '.'))) continue;
					
					string subPath = path.Replace((Namespace + "." + Name).Replace('/', '.'), "");
					
					int lastIndex = subPath.LastIndexOf('.');
					if(lastIndex > 0) {
						subPath = subPath[..lastIndex].Replace(".", "/") + subPath[lastIndex..];
					}
					
					if(subPath.StartsWith('.')) continue;

					string fullPath = tempDirPath + subPath;
					var resource = new Resource(ResourceType.CUSTOM, Namespace, Name + subPath.Replace('/', '.'), Assembly);

					var data = resource.ReadBytes();
					if(data == null) throw new IOException("Could not read resource data");

					Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
					File.Create(fullPath).Write(data);
				}

				return tempDirPath;
			} else {
				string tempFilePath = Path.GetTempFileName();

				var data = ReadBytes();
				if(data == null) throw new IOException("Could not read resource data");
				
				File.WriteAllBytes(tempFilePath, data);

				return tempFilePath;
			}
		}

		public override string ToString() => UID;
		
		public static bool operator==(Resource res1, IResource res2) => res1.UID == res2.UID;
		public static bool operator!=(Resource res1, IResource res2) => res1.UID != res2.UID;
	}
}