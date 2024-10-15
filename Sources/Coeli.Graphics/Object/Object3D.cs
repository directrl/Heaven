using System.Numerics;
using Coeli.Graphics.OpenGL;

namespace Coeli.Graphics.Object {
	
	public class Object3D : ObjectBase {

		public Vector3 Position = new();
		public Vector3 Rotation = new();
		public Vector3 Scale = new(1, 1, 1);
		
		public override Matrix4x4 ModelMatrix {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale);
				var positionMatrix = Matrix4x4.CreateTranslation(Position);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}
	}
}