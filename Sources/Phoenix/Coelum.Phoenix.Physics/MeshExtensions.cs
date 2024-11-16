using System.Numerics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Coelum.LanguageExtensions;
using Silk.NET.OpenGL;
using BepuMesh = BepuPhysics.Collidables.Mesh;

namespace Coelum.Phoenix.Physics {
	
	public static class MeshExtensions {

		// TODO
		public static BepuMesh CreatePhysicsMesh(this Mesh mesh, int resolution = 10) {
			if(mesh.Type != PrimitiveType.Triangles) {
				throw new ArgumentException("Physics meshes can be computed only for triangle meshes");
			}
			
			PhysicsGlobals.BufferPool.Take<Triangle>(mesh.Indices.Length / (3 * resolution),
			                                         out var triangles);

			int ti = 0;
			for(int i = 0; i < mesh.Indices.Length; i += 3 * resolution) {
				if(i > mesh.Indices.Length) break;
				
				var i0 = mesh.Indices[i];
				var i1 = mesh.Indices[i + 1];
				var i2 = mesh.Indices[i + 1];
				
				var v1 = mesh.Vertices[i0];
				var v2 = mesh.Vertices[i1];
				var v3 = mesh.Vertices[i2];
				
				triangles[ti] = new(v1.Position, v2.Position, v3.Position);
				ti++;
			}

			return new(triangles, Vector3.One, PhysicsGlobals.BufferPool);
		}
	}
}