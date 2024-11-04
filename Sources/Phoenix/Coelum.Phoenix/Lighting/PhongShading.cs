using Coelum.Debug;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.ECS.Nodes;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Lighting {
	
	public static class PhongShading {
		
		public static readonly ShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};
		
		public class FragmentShaderOverlay : ShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public override string Name => "lighting_phong";
			public override string Path => "Overlays.Lighting.Phong";
			
			public override ShaderType Type => ShaderType.FragmentShader;
			public override ShaderPass Pass => ShaderPass.COLOR_PRE;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;

			public override void Include(ShaderProgram shader) {
				Tests.Assert(shader.HasOverlays(SceneEnvironment.OVERLAYS),
					"Lighting shaders require SceneEnvironment overlays to be present");
			}
		}
	}
}