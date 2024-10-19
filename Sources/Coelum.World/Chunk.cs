using System.Numerics;
using Coelum.Debug;
using Coelum.Graphics;
using Coelum.Graphics.Node;
using Coelum.Graphics.Object;
using Coelum.World.Object;
using Silk.NET.Maths;

namespace Coelum.World {
	
	public class Chunk : SpatialNodeInstanced<WorldObject> {

		public const byte MAX_X = byte.MaxValue;
		public const byte MAX_Y = byte.MaxValue;
		public const byte MAX_Z = byte.MaxValue;

		public const int SIZE_X = MAX_X + 1;
		public const int SIZE_Y = MAX_Y + 1;
		public const int SIZE_Z = MAX_Z + 1;
		
		public const int SECTIONS_X = SIZE_X / Section.SIZE_X;
		public const int SECTIONS_Y = SIZE_Y / Section.SIZE_Y;
		public const int SECTIONS_Z = SIZE_Z / Section.SIZE_Z;

		public World Parent { get; }
		
		/// <summary>
		/// Position inside the parent world
		/// </summary>
		public Vector3D<int> Coordinates { get; }

		public Section?[,,] Sections { get; } = new Section[SECTIONS_Y, SECTIONS_Z, SECTIONS_X];

		public Chunk(World parent, Vector3D<int> coordinates) {
			Parent = parent;
			Coordinates = coordinates;
			Model = VoxelObject.MODEL;
		}

		public Section? GetSection(Vector3D<byte> coords) => Sections[coords.Y, coords.Z, coords.X];
		public void SetSection(Vector3D<byte> coords, Section section)
			=> Sections[coords.Y, coords.Z, coords.X] = section;

		public class Section {

			public const byte MAX_X = 63;
			public const byte MAX_Y = 3;
			public const byte MAX_Z = 63;

			public const byte SIZE_X = MAX_X + 1;
			public const byte SIZE_Y = MAX_Y + 1;
			public const byte SIZE_Z = MAX_Z + 1;

			public const int OBJECTS_X = SIZE_X;
			public const int OBJECTS_Y = SIZE_Y;
			public const int OBJECTS_Z = SIZE_Z;

			public Chunk Parent { get; }

			/// <summary>
			/// Position inside the parent chunk
			/// </summary>
			public Vector3D<byte> Coordinates { get; }

			public WorldObject?[,,] Objects { get; }

			public Section(Chunk parent, Vector3D<byte> coordinates) {
				Parent = parent;
				Coordinates = coordinates;
				Objects = new WorldObject[SIZE_Y, SIZE_Z, SIZE_X];
			}

			public Section(Chunk parent, Vector3D<byte> coordinates, WorldObject[,,] objects) {
				Tests.Assert(objects.GetLength(0) == SIZE_Y);
				Tests.Assert(objects.GetLength(1) == SIZE_Z);
				Tests.Assert(objects.GetLength(2) == SIZE_X);

				Parent = parent;
				Coordinates = coordinates;
				Objects = objects;
			}

			public WorldObject? GetObject(Vector3D<byte> position)
				=> Objects[position.Y, position.Z, position.X];
			
			public void SetObject(Vector3D<byte> position, WorldObject obj) {
				Objects[position.Y, position.Z, position.X] = obj;
				Parent.Add(obj);
			}

		public bool IsEmpty() {
				bool empty = true;

				for(int y = 0; y < SIZE_Y; y++) {
					Parallel.For(0, SIZE_X, (x, state) => {
						for(int z = 0; z < SIZE_Z; z++) {
							if(Objects[x, y, z] != null || !empty) {
								empty = false;
								state.Stop();
								return;
							}
						}
					});
				}
				
				return empty;
			}
		}
	}
}