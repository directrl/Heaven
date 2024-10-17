using Coeli.Graphics.Camera;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Scene {
	
	public class Scene2D : SceneBase {
		
		protected ShaderProgram MainShader { get; set; }
		public Camera2D? Camera { get; set; }
		
		public Scene2D(string id) : base(id) { }

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
			gl.Disable(EnableCap.CullFace);
			
			base.OnRender(gl, delta);
			
			MainShader.Bind();
			Camera?.Load(MainShader);
		}
	}
}