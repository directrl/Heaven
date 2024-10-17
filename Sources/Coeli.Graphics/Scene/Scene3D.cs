using Coeli.Graphics.Camera;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Scene {
	
	public class Scene3D : SceneBase {
		
		protected ShaderProgram MainShader { get; set; }
		public Camera3D? Camera { get; set; }
		
		protected Scene3D(string id) : base(id) { }

		public override void OnLoad(Window window) {
			base.OnLoad(window);

			MainShader = new(Module.RESOURCES,
				new(ShaderType.FragmentShader,
					Module.RESOURCES[Resource.Type.SHADER, "scene.frag"].ReadString()),
				new(ShaderType.VertexShader,
					Module.RESOURCES[Resource.Type.SHADER, "scene.vert"].ReadString())
			);
			MainShader.Validate();
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			
			MainShader.Bind();
			MainShader.SetUniform("texSampler", 0);
			MainShader.SetUniform("texArraySampler", 1);
			
			Camera?.Load(MainShader);
		}
	}
}