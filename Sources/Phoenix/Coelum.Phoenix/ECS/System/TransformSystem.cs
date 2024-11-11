using System.Numerics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;

namespace Coelum.Phoenix.ECS.System {
	
	public class TransformSystem : EcsSystem {

		public TransformSystem() : base("Object Transform") {
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Transform>()
			    .Parallel(true)
			    .Each((node, t) => {
				    switch(t) {
					    case Transform2D t2d:
						    var matrix = new Matrix4x4();

						    matrix.ComposeFromComponents(
							    new(t2d.Position, 1),
							    new(1, 1, t2d.Rotation),
							    new(t2d.Scale, 1)
						    );
						    
						    t2d.LocalMatrix = matrix;
						    
						    break;
					    case Transform3D t3d:
						    matrix = new();
						    
						    matrix.ComposeFromComponents(
							    t3d.Position,
							    t3d.Rotation,
							    t3d.Scale
						    );

						    t3d.LocalMatrix = matrix;
						    
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