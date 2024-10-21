using Coelum.Graphics.Camera;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Scene {
	
	public class Scene2D : SceneBase {
		
		public Camera2D? Camera { get; set; }

		protected Scene2D(string id) : base(id) {
			PrimaryShaderSetup += shader => {
				Camera?.Render(shader);
			};
		}

		public override void OnLoad(Window window) {
			PrimaryShader = new(
				Module.RESOURCES, 
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[ResourceType.SHADER, "scene.vert"])
			);
			
			base.OnLoad(window);
		}
		
		public override void OnRender(float delta) {
			Gl.Disable(EnableCap.CullFace);
			base.OnRender(delta);
		}
	}
}