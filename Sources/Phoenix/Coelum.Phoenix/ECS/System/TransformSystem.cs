using System.Numerics;
using Coelum.ECS;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.ECS.System {
	
	public class TransformSystem : EcsSystem {

		public TransformSystem() : base("Transform") {
			Action = Invoke;
		}

		private void Invoke(NodeRoot root, float delta) {
			root.Query<Transform>()
			    .Parallel(true)
			    .Each((node, t) => {
				    //if(!t.Dirty) return;

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
				    if(p != null) {
					    if(!p.HasComponent<Transform>()) {
						    t.GlobalMatrix = t.LocalMatrix;
						    t.Dirty = false;
					    }
					    
					    var pt = p.GetComponent<Transform>();
					    
					    //if(!pt.Dirty) {
						    t.GlobalMatrix = t.LocalMatrix * pt.GlobalMatrix;
						    t.Dirty = false;
					    //}
				    } else {
					    t.GlobalMatrix = t.LocalMatrix;
					    t.Dirty = false;
				    }
			    })
			    .Execute();
		}
	}
}