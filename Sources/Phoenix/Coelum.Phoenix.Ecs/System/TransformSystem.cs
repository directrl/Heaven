using System.Numerics;
using Coelum.Common.Ecs;
using Coelum.Phoenix.Ecs.Component;
using Flecs.NET.Core;

namespace Coelum.Phoenix.Ecs.System {
	
	public class TransformSystem : IEcsSystem {

		public static System<Transform> Create(World world) {
			return world.System<Transform>("Transform")
			            .Kind(FlEcs.PostUpdate)
			            .Each((Entity e, ref Transform t) => {
				            if(!t.Dirty) return;
				            
				            switch(t) {
					            case Transform2D t2d:
						            t.LocalMatrix =
							            Matrix4x4.CreateScale(t2d.Scale.X, t2d.Scale.Y, 1)
							            * Matrix4x4.CreateTranslation(t2d.Position.X, t2d.Position.Y, 1)
							            * Matrix4x4.CreateFromYawPitchRoll(1, 1, t2d.Rotation);
						            break;
					            case Transform3D t3d:
						            t.LocalMatrix =
							            Matrix4x4.CreateScale(t3d.Scale)
							            * Matrix4x4.CreateTranslation(t3d.Position)
							            * Matrix4x4.CreateFromYawPitchRoll(t3d.Rotation.Y, t3d.Rotation.X, t3d.Rotation.Z);
						            break;
				            }
				            
				            var p = e.Parent();
				            
				            if(p.IsValid()) {
					            var pT = p.Get<Transform>();
					            t.GlobalMatrix = t.LocalMatrix * pT.GlobalMatrix;
				            } else {
					            t.GlobalMatrix = t.LocalMatrix;
				            }

				            var tt = t;
				            
				            e.Children((Entity c) => {
					            if(!c.Has<Transform>()) return;
					            var cT = c.Get<Transform>();

					            cT.GlobalMatrix = cT.LocalMatrix * tt.GlobalMatrix;
				            });
				            
				            // t.GlobalMatrix = e.GetGlobal(
					           //  (Transform t) => t.LocalMatrix,
					           //  (arg1, arg2) => arg1 * arg2
				            // );

				            t.Dirty = false;
			            });
		}
	}
}