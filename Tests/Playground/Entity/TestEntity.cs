using Coelum.Graphics;
using Coelum.Graphics.Texture;
using Coelum.World.Components;
using Coelum.World.Entity;
using Silk.NET.OpenGL;

namespace Playground.Entity {

	public class TestEntity : Entity3D, IInteractable, ITickable {

		private static readonly Random RANDOM = new();
		
		public override Type[] Components => new[] {
			typeof(IInteractable)
		};

		public TestEntity() {
			Model = new(
				new Mesh(
					PrimitiveType.Triangles,
					new float[] {
						-0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, 0.5f, -0.5f,
						0.5f, 0.5f, -0.5f,
						-0.5f, 0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,

						-0.5f, -0.5f, 0.5f,
						0.5f, -0.5f, 0.5f,
						0.5f, 0.5f, 0.5f,
						0.5f, 0.5f, 0.5f,
						-0.5f, 0.5f, 0.5f,
						-0.5f, -0.5f, 0.5f,

						-0.5f, 0.5f, 0.5f,
						-0.5f, 0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, -0.5f,
						-0.5f, -0.5f, 0.5f,
						-0.5f, 0.5f, 0.5f,

						0.5f, 0.5f, 0.5f,
						0.5f, 0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, 0.5f,
						0.5f, 0.5f, 0.5f,

						-0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, -0.5f,
						0.5f, -0.5f, 0.5f,
						0.5f, -0.5f, 0.5f,
						-0.5f, -0.5f, 0.5f,
						-0.5f, -0.5f, -0.5f,

						-0.5f, 0.5f, -0.5f,
						0.5f, 0.5f, -0.5f,
						0.5f, 0.5f, 0.5f,
						0.5f, 0.5f, 0.5f,
						-0.5f, 0.5f, 0.5f,
						-0.5f, 0.5f, -0.5f,
					},
					new uint[] {
						0, 1, 2, 3, 4, 5,
						6, 7, 8, 9, 10, 11,
						12, 13, 14, 15, 16, 17,
						18, 19, 20, 21, 22, 23,
						24, 25, 26, 27, 28, 29,
						30, 31, 32, 33, 34, 35
					},
					new float[] {
						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,

						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,

						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,

						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,

						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,

						0, 0,
						1, 0,
						1, 1,
						1, 1,
						0, 1,
						0, 0,
					}, null
				)
			) {
				Material = new() {
					Texture = Texture2D.DefaultTexture
				}
			};
		}

		public void Interact() {
			Console.WriteLine(Id);
		}

		private bool _flag = false;
		private float _amount = 30.0f;
		public void Update(float delta) {
			Position.X += delta * (_flag ? -_amount : _amount);
			Position.Y += delta * (_flag ? -_amount : _amount);
			Position.Z += delta * (_flag ? -_amount : _amount);

			_flag = !_flag;
		}
	}
}