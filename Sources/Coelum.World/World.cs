using System.Numerics;
using Coelum.Graphics;
using Coelum.Graphics.Components;
using Coelum.Graphics.Node;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Coelum.World.Components;
using Coelum.World.Entity;
using Coelum.World.Object;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace Coelum.World {
	
	public class World : NodeBase, IRenderable, IShaderLoadable {

		public Dictionary<ulong, WorldEntity> Entities { get; } = new();
		public Dictionary<Vector3D<int>, Chunk> Chunks { get; } = new();
		
		private ulong _currentEntityId = 0;
		public ulong CurrentEntityId => _currentEntityId++;

		public void Spawn(WorldEntity entity) {
			entity.Id = CurrentEntityId;
			entity.World = this;

			Entities[entity.Id] = entity;
		}

		public GL GL { get; } = GLManager.Current;

		public void Build() {
			foreach(var chunk in Chunks.Values) {
				chunk.Build();
			}
		}
		
		public void Render() {
			foreach(var chunk in Chunks.Values) {
				chunk.Render();
			}
		}
		
		public void Load(ShaderProgram shader) {
			foreach(var chunk in Chunks.Values) {
				chunk.Load(shader);
			}
		}

		public Chunk CreateChunk(Vector3D<int> coords) {
			if(Chunks.TryGetValue(coords, out var existing)) return existing;
			
			var chunk = new Chunk(this, coords);
			Chunks[coords] = chunk;
			return chunk;
		}

		public TWorldObject? GetObject<TWorldObject>(WorldCoord coords)
			where TWorldObject : WorldObject {

			var chunkCoords = coords.ChunkCoordinates;
			var sectionCoords = coords.SectionCoordinates;
			var sectionPos = coords.SectionPosition;

			if(!Chunks.TryGetValue(chunkCoords, out var chunk)) return null;

			var section = chunk.GetSection(sectionCoords);

			if(section == null) {
				section = new(chunk, sectionCoords);
				chunk.SetSection(sectionCoords, section);
			}

			return (TWorldObject?) section.GetObject(sectionPos);
		}
		
		public void PlaceObject<TWorldObject>(TWorldObject obj)
			where TWorldObject : WorldObject {

			var coords = obj.Coordinates;

			var chunkCoords = coords.ChunkCoordinates;
			var sectionCoords = coords.SectionCoordinates;
			var sectionPos = coords.SectionPosition;

			if(!Chunks.TryGetValue(chunkCoords, out var chunk)) {
				throw new ArgumentOutOfRangeException(nameof(coords.ChunkCoordinates),
				                                      "Chunk does not exist");
			}

			var section = chunk.GetSection(sectionCoords);

			if(section == null) {
				section = new(chunk, sectionCoords);
				chunk.SetSection(sectionCoords, section);
			}

			section.SetObject(sectionPos, obj);
		}
	}
}