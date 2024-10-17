using Coeli.Graphics.Camera;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Scene {
	
	public class Scene2D : SceneBase {
		
		public Camera2D? Camera { get; set; }

		protected Scene2D(string id) : base(id) {
			PrimaryShaderSetup += (gl, shader) => {
				shader.SetUniform("texSampler", 0);
				shader.SetUniform("texArraySampler", 1);
				
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
		
		public override void OnRender(GL gl, float delta) {
			gl.Disable(EnableCap.CullFace);
			
			base.OnRender(gl, delta);
		}
	}
}