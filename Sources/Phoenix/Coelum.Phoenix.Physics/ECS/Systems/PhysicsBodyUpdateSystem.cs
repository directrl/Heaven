using System.Numerics;
using BepuPhysics;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Physics.ECS.Components;
using Hexa.NET.ImGuizmo;
using Serilog;

namespace Coelum.Phoenix.Physics.ECS.Systems {
	
	public class PhysicsBodyUpdateSystem : EcsSystem {

		public Simulation Simulation { get; }
		public PhysicsStore PhysicsStore { get; }
		
		public PhysicsBodyUpdateSystem(Simulation simulation) : base("Physics Body Update") {
			Simulation = simulation;
			PhysicsStore = simulation.GetStore();
			
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<PhysicsBody, Transform>()
			    .Each((node, p, t) => {
				    if(!p.DoUpdates) return;
				    if(t is not Transform3D t3d) return;

				    // TODO move to extension method or Transform3D itself
				    void UpdateTransform(Vector3 position, Quaternion orientation) {
					    var newMatrix = Matrix4x4.CreateFromQuaternion(orientation)
						    * Matrix4x4.CreateTranslation(position);

					    var translationMatrix = new Matrix4x4();
					    var rotationMatrix = new Matrix4x4();
					    var scaleMatrix = new Matrix4x4();
					    
					    ImGuizmo.DecomposeMatrixToComponents(
						    ref newMatrix,
						    ref translationMatrix,
						    ref rotationMatrix,
						    ref scaleMatrix
						);
					    
					    var translation = new Vector3(translationMatrix.M11, translationMatrix.M12, translationMatrix.M13);
					    var rotation = new Vector3(rotationMatrix.M11.ToRadians(), rotationMatrix.M12.ToRadians(), rotationMatrix.M13.ToRadians());

					    t3d.Position = translation;
					    t3d.Rotation = rotation;
				    }

				    switch(p) {
					    case StaticPhysicsBody body:
						    if(!PhysicsStore.GetStaticHandle(body, out var staticHandle)) {
							    var desc = new StaticDescription(
								    t3d.GlobalPosition,
								    t3d.QGlobalRotation,
								    body.ComputeShape().Index
							    );
							    
							    PhysicsStore.SetStatic(body, desc);
						    } else {
							    Simulation.Statics.GetDescription(staticHandle, out var desc);
							    UpdateTransform(desc.Pose.Position, desc.Pose.Orientation);
						    }
						    
						    break;
					    case DynamicPhysicsBody body:
						    if(!PhysicsStore.GetBodyHandle(body, out var dynamicHandle)) {
							    var shape = body.ComputeShape();

							    var desc = BodyDescription.CreateDynamic(
								    (t3d.GlobalPosition, t3d.QGlobalRotation),
								    shape.Inertia,
								    shape.Index,
								    0.01f
							    );
							    
							    PhysicsStore.SetBody(body, desc);
						    } else {
							    Simulation.Bodies.GetDescription(dynamicHandle, out var desc);
							    UpdateTransform(desc.Pose.Position, desc.Pose.Orientation);
						    }
						    
						    break;
					    case KinematicPhysicsBody body:
						    if(!PhysicsStore.GetBodyHandle(body, out var kinematicHandle)) {
							    var desc = BodyDescription.CreateKinematic(
								    (t3d.GlobalPosition, t3d.QGlobalRotation),
								    body.ComputeShape().Index,
								    0.01f
							    );
							    
							    PhysicsStore.SetBody(body, desc);
						    } else {
							    Simulation.Bodies.GetDescription(kinematicHandle, out var desc);
							    UpdateTransform(desc.Pose.Position, desc.Pose.Orientation);
						    }
						    
						    break;
				    }
			    })
			    .Execute();
		}
	}
}
