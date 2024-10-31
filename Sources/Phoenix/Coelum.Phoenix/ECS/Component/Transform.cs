using System.Numerics;
using Coelum.ECS;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform : NodeComponent {
		
		public bool Dirty { get; set; }
		
		public Matrix4x4 LocalMatrix { get; internal set; }
		public Matrix4x4 GlobalMatrix { get; internal set; }
	}
}