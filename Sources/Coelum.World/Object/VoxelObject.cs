using System.Numerics;
using Coelum.Graphics;
using Coelum.Graphics.Texture;
using Silk.NET.OpenGL;

namespace Coelum.World.Object {
	
	public class VoxelObject : WorldObject {

		public static readonly Model MODEL = 
			new(
				new Mesh(
					PrimitiveType.Triangles, 
					new float[] {
			             // VO
			             0f, 1f, 1f,
			             // V1
			             0f, 0f, 1f,
			             // V2
			             1f, 0f, 1f,
			             // V3
			             1f, 1f, 1f,
			             // V4
			             0f, 1f, 0f,
			             // V5
			             1f, 1f, 0f,
			             // V6
			             0f, 0f, 0f,
			             // V7
			             1f, 0f, 0f,
		             },
		             new uint[] {
			             // Front face
			             0, 1, 3, 3, 1, 2,
			             // Top Face
			             4, 0, 3, 5, 4, 3,
			             // Right face
			             3, 2, 7, 5, 3, 7,
			             // Left face
			             6, 1, 0, 6, 0, 4,
			             // Bottom face
			             2, 1, 6, 2, 6, 7,
			             // Back face
			             7, 6, 4, 7, 4, 5,
		             },
		             null,
		             null
				)
			);

		public override Type[] Components => Array.Empty<Type>();

		public override Model? Model => MODEL;

		public VoxelObject(World world, Chunk chunk, WorldCoord coords) : base(world, chunk, coords) { }
	}
}