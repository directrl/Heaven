using System.Numerics;
using Coelum.Graphics;
using Coelum.Graphics.Texture;
using Silk.NET.OpenGL;

namespace Coelum.World.Object {
	
	public class VoxelObject : WorldObject {
		
		public static readonly Model MODEL = new(
			new Mesh(
				PrimitiveType.Triangles,
				new float[] {
					-0.5f, -0.5f, -0.5f,
					0.5f, -0.5f, -0.5f,
					0.5f, 0.5f, -0.5f,
					0.5f, 0.5f, -0.5f,
					-0.5f, 0.5f, -0.5f,
					-0.5f, -0.5f, -0.5f,

					-0.5f, -0.5f, 0.5f,
					0.5f, -0.5f, 0.5f,
					0.5f, 0.5f, 0.5f,
					0.5f, 0.5f, 0.5f,
					-0.5f, 0.5f, 0.5f,
					-0.5f, -0.5f, 0.5f,

					-0.5f, 0.5f, 0.5f,
					-0.5f, 0.5f, -0.5f,
					-0.5f, -0.5f, -0.5f,
					-0.5f, -0.5f, -0.5f,
					-0.5f, -0.5f, 0.5f,
					-0.5f, 0.5f, 0.5f,

					0.5f, 0.5f, 0.5f,
					0.5f, 0.5f, -0.5f,
					0.5f, -0.5f, -0.5f,
					0.5f, -0.5f, -0.5f,
					0.5f, -0.5f, 0.5f,
					0.5f, 0.5f, 0.5f,

					-0.5f, -0.5f, -0.5f,
					0.5f, -0.5f, -0.5f,
					0.5f, -0.5f, 0.5f,
					0.5f, -0.5f, 0.5f,
					-0.5f, -0.5f, 0.5f,
					-0.5f, -0.5f, -0.5f,

					-0.5f, 0.5f, -0.5f,
					0.5f, 0.5f, -0.5f,
					0.5f, 0.5f, 0.5f,
					0.5f, 0.5f, 0.5f,
					-0.5f, 0.5f, 0.5f,
					-0.5f, 0.5f, -0.5f,
				},
				new uint[] {
					0, 1, 2, 3, 4, 5,
					6, 7, 8, 9, 10, 11,
					12, 13, 14, 15, 16, 17,
					18, 19, 20, 21, 22, 23,
					24, 25, 26, 27, 28, 29,
					30, 31, 32, 33, 34, 35
				},
				new float[] {
					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,

					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,

					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,

					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,

					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,

					0, 0,
					1, 0,
					1, 1,
					1, 1,
					0, 1,
					0, 0,
				}, null
			)
		) {
			Material = new() {
				Texture = Texture2D.DefaultTexture
			}
		};

		public override Type[] Components => Array.Empty<Type>();

		public override Model? Model => MODEL;

		public VoxelObject(World world, Chunk chunk, Coord coordinates) : base(world, chunk, coordinates) { }
	}
}