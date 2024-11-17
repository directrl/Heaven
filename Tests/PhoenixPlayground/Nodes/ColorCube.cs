using System.Drawing;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Components;
using Silk.NET.OpenGL;
using Node = Coelum.ECS.Node;

namespace PhoenixPlayground.Nodes {
	
	public class ColorCube : Node {

		public static readonly Model DEFAULT_MODEL = new("color_cube") {
			Meshes = new() {
				new(PrimitiveType.Triangles,
					new Vertex[] {
						new(new(-0.5f, 0.5f, 0.5f), new(), new()),
						new(new(-0.5f, -0.5f, 0.5f), new(), new()),
						new(new(0.5f, -0.5f, 0.5f), new(), new()),
						new(new(0.5f, 0.5f, 0.5f), new(), new()),
						new(new(-0.5f, 0.5f, -0.5f), new(), new()),
						new(new(0.5f, 0.5f, -0.5f), new(), new()),
						new(new(-0.5f, -0.5f, -0.5f), new(), new()),
						new(new(0.5f, -0.5f, -0.5f), new(), new()),
					},
					new uint[] {
						// Front face
						0, 1, 3, 3, 1, 2,
						// Top Face
						4, 0, 3, 5, 4, 3,
						// Right face
						3, 2, 7, 5, 3, 7,
						// Left face
						6, 1, 0, 6, 0, 4,
						// Bottom face
						2, 1, 6, 2, 6, 7,
						// Back face
						7, 6, 4, 7, 4, 5,
					})
				{
					MaterialIndex = 0
				}
			}
		};
		
		public ColorCube() { }

		public ColorCube(Color color) {
			var model = new Model(DEFAULT_MODEL) {
				Materials = new() {
					new() {
						Albedo = color.ToVector4()
					}
				}
			};

			AddComponent<Renderable>(new ModelRenderable(model));
			AddComponent<Transform>(new Transform3D());
		}
	}
}