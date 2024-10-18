using Coelum.LanguageExtensions;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Texture {
	
	class TextureCacheBase<TTexture> : CacheBase<TextureCacheBase<TTexture>.Entry, TTexture> {

		public readonly struct Entry : IEquatable<Entry> {
				
			public GL GL { get; init; }
			public string UID { get; init; }

			public override string ToString() {
				return $"TextureCacheBase.Entry{{{UID}}}";
			}

			public bool Equals(Entry other) {
				return UID == other.UID;
			}

			public static bool operator==(Entry e1, Entry e2) {
				return e1.Equals(e2);
			}
		
			public static bool operator!=(Entry e1, Entry e2) {
				return !e1.Equals(e2);
			}
		}
	}
}