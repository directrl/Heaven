using System.Numerics;
using Coelum.Graphics.Node;
using Silk.NET.Maths;

namespace Coelum.World.Object {
	
	public abstract class WorldObject : Node3D {
		
		// world object transforms are immutable
		public new Vector3 Position {
			get => base.Position;
			init => base.Position = value;
		}
		
		public new Vector3 Rotation {
			get => base.Rotation;
			init => base.Rotation = value;
		}
		
		public new Vector3 Scale {
			get => base.Scale;
			init => base.Scale = value;
		}
		
		public World World { get; internal set; }
		public Chunk Chunk { get; internal set; }
		
		public Coord Coordinates { get; }

		public abstract Type[] Components { get; }

		public WorldObject(World world, Chunk chunk, Coord coordinates) {
			World = world;
			Chunk = chunk;
			Coordinates = coordinates;
			
			base.Position = new(coordinates.WorldPosition.X,
			                    coordinates.WorldPosition.Y,
			                    coordinates.WorldPosition.Z);
		}
		
		public struct Coord {

			public Vector3D<int> WorldPosition { get; set; }

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

			public Coord() {
				WorldPosition = new();
			}

			public Coord(int x, int y, int z) {
				WorldPosition = new(x, y, z);
			}
		}
	}
}