using Coelum.ECS;
using Coelum.ECS.Prefab;
using Coelum.Phoenix.Camera;

namespace Coelum.Phoenix.Editor.Prefabs {
	
	[Prefab("Perspective Camera 3D")]
	public class PerspectiveCamera3D : Node, IPrefab {

		public Camera3D Camera { get; init; }
		
		public Node Create(NodeRoot root) {
			if(root is not PhoenixScene scene) {
				throw new ArgumentException("root must be a PhoenixScene", nameof(root));
			}
			
			return new PerspectiveCamera3D() {
				Camera = new PerspectiveCamera(scene.Window)
			};
		}
	}
}