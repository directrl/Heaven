using Coelum.Graphics.Camera;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Scene {
	
	public class Scene3D : SceneBase {
		
		public Camera3D? Camera { get; set; }

		protected Scene3D(string id) : base(id) {
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
	}
}