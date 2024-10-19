using Silk.NET.Maths;

namespace Coelum.World {
	
	public struct WorldCoord {

		public Vector3D<int> WorldPosition;

		public int X {
			get => WorldPosition.X;
			set => WorldPosition.X = value;
		}
			
		public int Y {
			get => WorldPosition.Y;
			set => WorldPosition.Y = value;
		}
			
		public int Z {
			get => WorldPosition.Z;
			set => WorldPosition.Z = value;
		}

		/// <summary>
		/// The position of the Coord inside a chunk
		/// </summary>
		public Vector3D<byte> ChunkPosition => new(
			(byte) (WorldPosition.X % Chunk.SIZE_X),
			(byte) (WorldPosition.Y % Chunk.SIZE_Y),
			(byte) (WorldPosition.Z % Chunk.SIZE_Z)
		);

		/// <summary>
		/// The position of the Coord inside a section
		/// </summary>
		public Vector3D<byte> SectionPosition => new(
			(byte) (ChunkPosition.X % Chunk.Section.SIZE_X),
			(byte) (ChunkPosition.Y % Chunk.Section.SIZE_Y),
			(byte) (ChunkPosition.Z % Chunk.Section.SIZE_Z)
		);
			
		/// <summary>
		/// The coordinates of the chunk the Coord belongs to
		/// </summary>
		public Vector3D<int> ChunkCoordinates => new(
			(int) MathF.Floor(WorldPosition.X / Chunk.SIZE_X),
			(int) MathF.Floor(WorldPosition.Y / Chunk.SIZE_Y),
			(int) MathF.Floor(WorldPosition.Z / Chunk.SIZE_Z)
		);
			
		/// <summary>
		/// The coordinates of the section the Coord belongs to
		/// </summary>
		public Vector3D<byte> SectionCoordinates => new(
			(byte) MathF.Floor(ChunkPosition.X / Chunk.Section.SIZE_X),
			(byte) MathF.Floor(ChunkPosition.Y / Chunk.Section.SIZE_Y),
			(byte) MathF.Floor(ChunkPosition.Z / Chunk.Section.SIZE_Z)
		);

		public WorldCoord() {
			WorldPosition = new();
		}

		public WorldCoord(int x, int y, int z) {
			WorldPosition = new(x, y, z);
		}

		public WorldCoord(Chunk chunk) {
			WorldPosition = new(
				chunk.Coordinates.X * Chunk.SIZE_X,
				chunk.Coordinates.Y * Chunk.SIZE_Y,
				chunk.Coordinates.Z * Chunk.SIZE_Z
			);
		}
	}
}