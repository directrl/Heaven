using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Trees;
using BepuUtilities.Memory;
using Coelum.Debug;
using Coelum.Phoenix.Camera;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.Physics.ECS.Nodes;

namespace Coelum.Phoenix.Physics {
	
	public static class RayCastHelper {

		public static THitHandler RayCast<THitHandler>(this Simulation simulation, 
		                                               Camera3D camera, Viewport viewport, Vector2 mousePos, 
		                                               float distance = 1000, 
		                                               IRayHitHandler? hitHandler = null)
			where THitHandler : struct, IRayHitHandler {

			var ndcMousePos = new Vector3(
				(2.0f * mousePos.X) / viewport.Framebuffer.Size.X - 1.0f,
				1.0f - (2.0f * mousePos.Y) / viewport.Framebuffer.Size.Y,
				-1.0f
			);

			var mouseDir = new Vector4(ndcMousePos.X, ndcMousePos.Y, ndcMousePos.Z, 1.0f);
			mouseDir = Vector4.Transform(mouseDir, camera.InverseProjectionMatrix);
			mouseDir.Z = -1.0f;
			mouseDir.W = 0.0f;

			mouseDir = Vector4.Transform(mouseDir, camera.InverseViewMatrix);
			//mouseDir = Vector4.Normalize(mouseDir);
			
			// var viewCoords = Vector4.Transform(clipCoords, camera.InverseProjectionMatrix);
			//
			// var rayDirectionView = new Vector3(viewCoords.X, viewCoords.Y, viewCoords.Z) / viewCoords.W;
			// var rayCoordsWorld = Vector4.Transform(new Vector4(rayDirectionView, 0.0f), camera.InverseViewMatrix);
			// var rayDirectionWorld = Vector3.Normalize(new(rayCoordsWorld.X, rayCoordsWorld.Y, rayCoordsWorld.Z));
			
			hitHandler ??= new SingleHitHandler(simulation);
			
			simulation.RayCast(
				camera.GetComponent<Transform3D>().GlobalPosition,
				new(mouseDir.X, mouseDir.Y, mouseDir.Z),
				distance,
				ref hitHandler
			);

			return (THitHandler) hitHandler;
		}
	}
	
	public struct RayHit {
			
		public Vector3 Normal;
		public float T;
		public CollidableReference Collidable;
		public PhysicsBody3D? PhysicsNode;
		public bool Hit;

		public override string ToString() {
			return $"{Normal}, {T}, {Collidable}, {Hit}";
		}
	}

	public struct SingleHitHandler : IRayHitHandler {

		private Simulation _simulation;
		public RayHit Result;

		public SingleHitHandler(Simulation simulation) {
			_simulation = simulation;
			Result = new();
		}
		
		public bool AllowTest(CollidableReference collidable) {
			return true;
		}
			
		public bool AllowTest(CollidableReference collidable, int childIndex) {
			return true;
		}
			
		public void OnRayHit(in RayData ray,
		                     ref float maximumT, float t,
		                     in Vector3 normal,
		                     CollidableReference collidable, int childIndex) {
				
			maximumT = t;
				
			Result.Normal = normal;
			Result.T = t;
			Result.Collidable = collidable;

			PhysicsBody3D? physicsNode = null;

			if(_simulation.Statics.StaticExists(collidable.StaticHandle)) {
				_simulation.GetStore().GetPhysicsNode(collidable.StaticHandle, out var node);
				physicsNode = node;
			} else if(_simulation.Bodies.BodyExists(collidable.BodyHandle)) {
				_simulation.GetStore().GetPhysicsNode(collidable.BodyHandle, out var node);
				physicsNode = node;
			}

			Result.PhysicsNode = physicsNode;
			Result.Hit = true;
		}
	}
}