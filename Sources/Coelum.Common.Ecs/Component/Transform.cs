using System.Numerics;

namespace Coelum.Common.Ecs.Component {

	public abstract class Transform {

		public abstract Matrix4x4 Matrix { get; }
	}
}