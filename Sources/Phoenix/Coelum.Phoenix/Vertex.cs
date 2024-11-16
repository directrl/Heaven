using System.Numerics;
using System.Runtime.InteropServices;

namespace Coelum.Phoenix {
	
	[StructLayout(LayoutKind.Explicit)]
	public struct Vertex {

		public const int POSITION_OFFSET = 0 * 12;
		public const int NORMAL_OFFSET = 1 * 12;
		public const int TEXCOORDS_OFFSET = 2 * 12;

		[FieldOffset(POSITION_OFFSET)]
		public Vector3 Position;
		
		[FieldOffset(NORMAL_OFFSET)]
		public Vector3 Normal;
		
		[FieldOffset(TEXCOORDS_OFFSET)]
		public Vector2 TexCoords;

		public Vertex(Vector3 position = new(),
		              Vector3 normal = new(),
		              Vector2 texCoords = new()) {
			
			Position = position;
			Normal = normal;
			TexCoords = texCoords;
		}
	}
}