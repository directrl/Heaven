using System.Text;
using Serilog;

namespace Coelum.Resources {
	
	public struct RawResource : IResource {
		
		public byte[] Data { get; }

		public string Name { get; }
		public ResourceType Type { get; }

		public string FullPath => throw new NotSupportedException();
		public string UID { get; }

		public bool Cache {
			get => false;
			set => throw new NotSupportedException();
		}

		public RawResource(ResourceType type, string name, Span<byte> data)
			: this(type, name, data.ToArray()) { }
		
		public RawResource(ResourceType type, string name, byte[] data) {
			Type = type;
			Name = name;
			Data = data;
			
			UID = $"RAW({type.Path}):" + name + type.Extension;
		}

		public Stream GetStream() {
			return new MemoryStream(Data);
		}

		public byte[] ReadBytes() {
			return Data.ToArray();
		}
		
		public string? ReadString(Encoding? encoding = null) {
			encoding ??= Encoding.UTF8;
			return encoding.GetString(ReadBytes());
		}

		public string Export() {
			string tempFilePath = Path.GetTempFileName();
			File.WriteAllBytes(tempFilePath, ReadBytes());

			return tempFilePath;
		}

		public override string ToString() => UID;
		
		public static bool operator==(RawResource res1, IResource res2) => res1.UID == res2.UID;
		public static bool operator!=(RawResource res1, IResource res2) => res1.UID != res2.UID;
	}
}