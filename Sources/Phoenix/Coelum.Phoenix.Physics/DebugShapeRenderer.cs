using System.Drawing;
using System.Numerics;
using BepuPhysics.Collidables;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.ModelLoading;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Serilog;
using Silk.NET.OpenGL;

using static Coelum.Phoenix.OpenGL.GlobalOpenGL;

namespace Coelum.Phoenix.Physics {
	
	public static class DebugShapeRenderer {
		
	#if DEBUG
		private static Dictionary<Node, (Transform3D t3d, Model model, Vector3 scale)> _models = new();
		
		public static Color OutlineColor { get; set; } = Color.OrangeRed;
		public static bool WireframeOnly { get; set; } = true;

		public static void Add(Node node, IShape shape) {
			if(_models.ContainsKey(node)) {
				Remove(node);
			}

			if(!node.TryGetComponent<Transform3D>(out var t3d)) {
				return;
			}

			Model? model = null;
			var scale = Vector3.One;

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
				case Sphere sphere:
					model = ModelLoader.Load(Module.RESOURCES[ResourceType.MODEL, "sphere.glb"]);
					if(model is not null) model.Materials[0].Albedo = OutlineColor.ToVector4();
					
					scale = new(sphere.Radius, sphere.Radius, sphere.Radius);
					break;
				case Cylinder cylinder:
					model = ModelLoader.Load(Module.RESOURCES[ResourceType.MODEL, "cylinder.glb"]);
					if(model is not null) model.Materials[0].Albedo = OutlineColor.ToVector4();
					
					scale = new(cylinder.Radius, cylinder.HalfLength, cylinder.Radius);
					break;
				default:
					Log.Warning($"[DebugShapeRenderer] No model for shape type {shape.GetType()}");
					break;
			}
			
			if(model is not null) _models[node] = (t3d, model, scale);
		}

		public static void Remove(Node node) {
			_models.Remove(node);
		}
		
		public static void Clear() {
			_models.Clear();
		}

		public static void Render(ShaderProgram shader) {
			if(WireframeOnly) Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);

			_models = _models
			          .Where(kv => kv.Key.Alive)
			          .ToDictionary(kv => kv.Key, kv => kv.Value);
			
			foreach(var (_, (t3d, model, scale)) in _models) {
				var matrix = new Matrix4x4();
				
				matrix.ComposeFromComponents(
					t3d.GlobalPosition,
					t3d.GlobalRotation,
					scale
				);
				
				shader.SetUniform("model", matrix);
				model.Render(shader);
			}
			
			Gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
		}
	#endif
	}
}