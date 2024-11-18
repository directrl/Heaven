using Coelum.ECS;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class LightingSystem : EcsSystem {
		
		private readonly ShaderProgram _shader;
		private readonly Lights _ubo;

		public LightingSystem(ShaderProgram shader) : base("Light Uniform Loader", SystemPhase.RENDER_PRE) {
			_shader = shader;
			_ubo = shader.CreateBufferBinding<Lights>();
			
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
						    light.Load(_ubo, directionalIndex++);
						    break;
					    case SpotLight:
						    light.Load(_ubo, spotIndex++);
						    break;
					    case PointLight:
						    light.Load(_ubo, pointIndex++);
						    break;
				    }
			    })
			    .Execute();

			_ubo.DirectionalLightCount = directionalIndex;
			_ubo.PointLightCount = pointIndex;
			_ubo.SpotLightCount = spotIndex;
			
			_ubo.Upload();
		}
	}
}