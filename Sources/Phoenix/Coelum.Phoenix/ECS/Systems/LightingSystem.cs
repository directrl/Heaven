using Coelum.ECS;
using Coelum.ECS.Queries;
using Coelum.Phoenix.ECS.Components;
using Coelum.Phoenix.OpenGL;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Systems {
	
	public class LightingSystem : ChildQuerySystem {
		
		private readonly Lights _ubo;

		private int _directionalIndex;
		private int _pointIndex;
		private int _spotIndex;
		
		public override string Name => "Light Uniform Loader";
		public override SystemPhase Phase => SystemPhase.RENDER_POST;

		public LightingSystem(ShaderProgram shader) {
			_ubo = shader.CreateBufferBinding<Lights>();

			Query = new ComponentQuery<Light>(Phase, QueryAction);
		}

		private void QueryAction(NodeRoot root, Light light) {
			switch(light) {
				case DirectionalLight:
					light.Load(_ubo, _directionalIndex++);
					break;
				case SpotLight:
					light.Load(_ubo, _spotIndex++);
					break;
				case PointLight:
					light.Load(_ubo, _pointIndex++);
					break;
			}
		}

		// TODO Note: this isn't ideal because Reset() is gonna get called on the next frame
		// which will introcude a single-frame delay
		public override void Reset() {
			base.Reset();
			
			_ubo.DirectionalLightCount = _directionalIndex;
			_ubo.PointLightCount = _pointIndex;
			_ubo.SpotLightCount = _spotIndex;
			
			_ubo.Upload();

			_directionalIndex = 0;
			_pointIndex = 0;
			_spotIndex = 0;
		}
	}
}