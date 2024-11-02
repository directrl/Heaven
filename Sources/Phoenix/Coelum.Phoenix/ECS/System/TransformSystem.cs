using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.ECS.System {
	
	public class TransformSystem : EcsSystem {

		public TransformSystem() : base("Transform") {
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Transform>()
			    .Parallel(true)
			    .Each((node, t) => {
				    switch(t) {
					    case Transform2D t2d:
						    t.LocalMatrix =
							    Matrix4x4.CreateScale(t2d.Scale.X, t2d.Scale.Y, 1)
							    * Matrix4x4.CreateFromYawPitchRoll(1, 1, t2d.Rotation)
							    * Matrix4x4.CreateTranslation(t2d.Position.X, t2d.Position.Y, 1);
						    break;
					    case Transform3D t3d:
						    t.LocalMatrix =
							    Matrix4x4.CreateScale(t3d.Scale)
							    * Matrix4x4.CreateFromYawPitchRoll(t3d.Rotation.Y, t3d.Rotation.X, t3d.Rotation.Z)
							    * Matrix4x4.CreateTranslation(t3d.Position);
						    break;
				    }

				    var p = node.Parent;
				    if(p != null && p.HasComponent<Transform>()) {
					    var pt = p.GetComponent<Transform>();
					    t.GlobalMatrix = t.LocalMatrix * pt.GlobalMatrix;
				    } else {
					    t.GlobalMatrix = t.LocalMatrix;
				    }
			    })
			    .Execute();
		}
	}
}