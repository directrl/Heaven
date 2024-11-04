using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Component;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.System {
	
	public class LightSystem : EcsSystem {
		
		private readonly ShaderProgram _shader;

		public LightSystem(ShaderProgram shader) : base("Light Uniform Loader") {
			_shader = shader;
			Action = ActionImpl;
		}

		private void ActionImpl(NodeRoot root, float delta) {
			root.Query<Transform, Light>()
			    .Each((node, t, light) => {
				    light.Load(_shader);
			    })
			    .Execute();
		}
	}
}