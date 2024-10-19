using System.Numerics;
using Coelum.Graphics.Node;
using Silk.NET.Maths;

namespace Coelum.World.Object {
	
	public abstract class WorldObject : Node3D {
		
		// world object transforms are immutable
		/*public new Vector3 Position {
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
		}*/
		
		public World World { get; internal set; }
		public Chunk Chunk { get; internal set; }
		
		public WorldCoord Coordinates { get; }

		public abstract Type[] Components { get; }

		public WorldObject(World world, Chunk chunk, WorldCoord coords) {
			World = world;
			Chunk = chunk;
			Coordinates = coords;
			
			Position = new(coords.WorldPosition.X,
			               coords.WorldPosition.Y,
			               coords.WorldPosition.Z);
		}
	}
}