using Coelum.Graphics.Camera;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Scene {
	
	public class Scene2D : SceneBase {
		
		public Camera2D? Camera { get; set; }

		protected Scene2D(string id) : base(id) {
			PrimaryShaderSetup += shader => {
				Camera?.Load(shader);
			};
		}

		public override void OnLoad(Window window) {
			PrimaryShader = new(
				Module.RESOURCES, 
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[Resource.Type.SHADER, "scene.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[Resource.Type.SHADER, "scene.vert"])
			);
			
			base.OnLoad(window);
		}
		
		public override void OnRender(float delta) {
			Gl.Disable(EnableCap.CullFace);
			base.OnRender(delta);
		}
	}
}