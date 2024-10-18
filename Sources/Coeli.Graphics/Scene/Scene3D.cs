using System.Runtime.InteropServices.ComTypes;
using Coeli.Graphics.Camera;
using Coeli.Graphics.OpenGL;
using Coeli.Resources;
using Silk.NET.OpenGL;

namespace Coeli.Graphics.Scene {
	
	public class Scene3D : SceneBase {
		
		public Camera3D? Camera { get; set; }

		protected Scene3D(string id) : base(id) {
			PrimaryShaderSetup += (gl, _, shader) => {
				//shader.SetUniform("texSampler", 0);
				//shader.SetUniform("texArraySampler", 1);
				
				Camera?.Load(shader);
			};
		}

		public override void OnLoad(Window window) {
			PrimaryShader = new(
				Module.RESOURCES,
				new(ShaderType.FragmentShader,
				    Module.RESOURCES[Resource.Type.SHADER, "scene_new.frag"]),
				new(ShaderType.VertexShader,
				    Module.RESOURCES[Resource.Type.SHADER, "scene_new.vert"])
			);
			
			base.OnLoad(window);
		}
	}
}