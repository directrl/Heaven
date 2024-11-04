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
			int directionalIndex = 0,
			    pointIndex = 0,
			    spotIndex = 0;
			
			root.Query<Transform, Light>()
			    .Each((node, t, light) => {
				    switch(light) {
					    case DirectionalLight:
						    light.Load(_shader, "directional_lights[" + directionalIndex++ + "]");
						    break;
					    case SpotLight sl:
						    light.Load(_shader, "spot_lights[" + spotIndex++ + "]");
						    break;
					    case PointLight pl:
						    light.Load(_shader, "point_lights[" + pointIndex++ + "]");
						    break;
				    }
			    })
			    .Execute();

			_shader.SetUniform("directional_light_count", directionalIndex);
			_shader.SetUniform("point_light_count", pointIndex);
			_shader.SetUniform("spot_light_count", spotIndex);
		}
	}
}