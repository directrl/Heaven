using System.Text;

namespace Coelum.Resources {
	
	public interface IResource : IEquatable<IResource> {
		
		public string Name { get; }
		public ResourceType Type { get; }
		
		public string UID { get; }
		
		public bool Cache { get; set; }

		public Stream? GetStream();
		public byte[]? ReadBytes();
		public string? ReadString(Encoding? encoding = null);

		bool IEquatable<IResource>.Equals(IResource? other) {
			return UID == (other?.UID ?? "");
		}
	}
}