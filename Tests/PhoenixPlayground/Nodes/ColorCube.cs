using System.Drawing;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix;
using Coelum.Phoenix.ECS.Component;
using Silk.NET.OpenGL;
using Node = Coelum.ECS.Node;

namespace PhoenixPlayground.Nodes {
	
	public class ColorCube : Node {

		private static readonly Model DEFAULT_MODEL = new("color_cube") {
			Meshes = new() {
				new(PrimitiveType.Triangles,
				    new float[] {
					    // VO
					    -0.5f, 0.5f, 0.5f,
					    // V1
					    -0.5f, -0.5f, 0.5f,
					    // V2
					    0.5f, -0.5f, 0.5f,
					    // V3
					    0.5f, 0.5f, 0.5f,
					    // V4
					    -0.5f, 0.5f, -0.5f,
					    // V5
					    0.5f, 0.5f, -0.5f,
					    // V6
					    -0.5f, -0.5f, -0.5f,
					    // V7
					    0.5f, -0.5f, -0.5f,
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
				    },
				    null,
				    null
				) {
					MaterialIndex = 0
				}
			}
		};

		public ColorCube(Color color) {
			var model = new Model() {
				Meshes = DEFAULT_MODEL.Meshes,
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