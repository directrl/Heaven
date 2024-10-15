using System.Numerics;
using Coeli.Graphics.OpenGL;
using Silk.NET.Maths;

namespace Coeli.Graphics.Object {
	
	public class Object2D : Model {

		public Vector2 Position = new();
		public Vector3 Rotation = new();
		public Vector2 Scale = new(1, 1);
		
		public Matrix4x4 ModelMatrix {
			get {
				var scaleMatrix = Matrix4x4.CreateScale(Scale.X, Scale.Y, 1);
				var positionMatrix = Matrix4x4.CreateTranslation(Position.X, Position.Y, 1);
				var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
				
				return scaleMatrix * positionMatrix * rotationMatrix;
			}
		}

		public void Render(ShaderProgram shader) {
			shader.SetUniform("model", ModelMatrix);
			base.Render(shader);
		}
	}
}