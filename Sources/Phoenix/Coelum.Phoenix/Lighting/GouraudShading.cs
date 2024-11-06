using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Lighting {
	
	public static class GouraudShading {

		public static readonly ShaderOverlay[] OVERLAYS = {
			VertexShaderOverlay.OVERLAY
		};
		
		public class VertexShaderOverlay : ShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public override string Name => "lighting_gouraud";
			public override string Path => "Overlays.Lighting.Gouraud";
			
			public override ShaderType Type => ShaderType.VertexShader;
			public override ShaderPass Pass => ShaderPass.RETURN;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
			
			public int MaxDirectionalLights { get; set; } = 4;
			public int MaxLights { get; set; } = 100;
			
			public override void Include(ShaderProgram shader) {
				Tests.Assert(shader.HasOverlays(SceneEnvironment.OVERLAYS),
				             "Lighting shaders require SceneEnvironment overlays to be present");
				
				shader.Define("MAX_DIRECTIONAL_LIGHTS", MaxDirectionalLights);
				shader.Define("MAX_LIGHTS", MaxLights);
			}
		}
	}
}