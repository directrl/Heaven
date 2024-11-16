using System.Drawing;
using System.Numerics;
using BepuPhysics.Collidables;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;
using Silk.NET.OpenGL;

using static Coelum.Phoenix.OpenGL.GlobalOpenGL;

namespace Coelum.Phoenix.Physics {
	
	public static class DebugShapeStore {
		
	#if DEBUG
		public static Dictionary<Node, IShape> Shapes = new();
		public static List<(Transform3D t3d, Model model)> Models = new();
		
		public static Color OutlineColor { get; set; } = Color.OrangeRed;

		public static void Add(Node node, IShape shape) {
			Shapes[node] = shape;

			if(!node.TryGetComponent<Transform3D>(out var t3d)) {
				return;
			}

			Model? model = null;

			switch(shape) {
				case Box box:
					var x = box.Width;
					var y = box.Height;
					var z = box.Length;
					
					model = new(null) {
						Meshes = new() {
							new(PrimitiveType.LineStrip,
							    new Vertex[] {
								    new(new(-x/2, -y/2, -z/2)),
								    new(new(x/2, -y/2, -z/2)),
								    new(new(x/2, -y/2, z/2)),
								    new(new(-x/2, -y/2, z/2)),

								    new(new(-x/2, y/2, -z/2)),
								    new(new(x/2, y/2, -z/2)),
								    new(new(x/2, y/2, z/2)),
								    new(new(-x/2, y/2, z/2))
							    },
							    new uint[] {
								    0, 1, 2, 3, 0,
								    4, 5, 6, 7, 4,
								    0, 1, 5, 2, 6, 3, 7
							    }
							)
						},
						Materials = new() {
							new() {
								Albedo = OutlineColor.ToVector4() // TODO use Color in Material instead
							}
						}
					};
					break;
			}
			
			if(model is not null) Models.Add((t3d, model));
		}
		
		public static void Clear() {
			Shapes.Clear();
			Models.Clear();
		}

		public static void Render(ShaderProgram shader) {
			Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
			
			foreach(var (t3d, model) in Models) {
				var matrix = new Matrix4x4();
				
				matrix.ComposeFromComponents(
					t3d.GlobalPosition,
					t3d.GlobalRotation,
					Vector3.One
				);
				
				shader.SetUniform("model", matrix);
				model.Render(shader);
			}
			
			Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
		}
	#endif
	}
}