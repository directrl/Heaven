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

			MainShader = new(
				new(ShaderType.FragmentShader,
					Module.RESOURCES[Resource.Type.SHADER, "scene3d_singlecolor.frag"].ReadString()),
				new(ShaderType.VertexShader,
					Module.RESOURCES[Resource.Type.SHADER, "scene3d_singlecolor.vert"].ReadString())
			);
			MainShader.Validate();
		}

		public override void OnRender(GL gl, float delta) {
			base.OnRender(gl, delta);
			
			MainShader.Bind();
			Camera?.Load(MainShader);
		}
	}
}