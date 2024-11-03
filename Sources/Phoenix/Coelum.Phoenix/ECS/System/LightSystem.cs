using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	public class LightSystem : EcsSystem {
		
		private ShaderProgram _shader;

		public LightSystem(ShaderProgram shader) : base("Light Uniform Loader") {
			_shader = shader;
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Transform, Light>()
			    .Each((node, t, light) => {
				    if(t is Transform3D t3d) {
					    // _shader.SetUniform("scene_env.light_pos", t3d.GlobalPosition);
					    // _shader.SetUniform("scene_env.light_color", light.Color.ToVector3());
					    
					    light.Load(_shader);
				    }
			    })
			    .Execute();
		}
	}
}