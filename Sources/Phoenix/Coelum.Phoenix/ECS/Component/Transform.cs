using System.Numerics;
using Coelum.ECS;

namespace Coelum.Phoenix.ECS.Component {
	
	public class Transform : INodeComponent {

		public Node? Owner { get; set; }

		public Matrix4x4 LocalMatrix { get; internal set; }
		public Matrix4x4 GlobalMatrix { get; internal set; }
	}
}