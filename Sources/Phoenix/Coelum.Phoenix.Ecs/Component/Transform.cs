using System.Numerics;
using Coelum.Common.Ecs;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs.Component {

	public abstract class Transform {

		public bool Dirty { get; set; } = true;
		
		public Matrix4x4 LocalMatrix { get; internal set; }
		public Matrix4x4 GlobalMatrix { get; internal set; }
		
		// public static Matrix4x4 GlobalMatrix(Entity e) {
		// 	return e.GetGlobal(
		// 		(Transform t) => t.LocalMatrix,
		// 		(arg1, arg2) => arg1 * arg2
		// 	);
		// }
	}
}