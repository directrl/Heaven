using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Lighting {
	
	public static class GouraudShading {

		public static readonly ShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY,
			VertexShaderOverlay.OVERLAY
		};
		
		public class FragmentShaderOverlay : ShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public override string Name => "lighting_gouraud";
			public override string Path => "Overlays.Lighting.Gouraud";
			
			public override ShaderType Type => ShaderType.FragmentShader;
			public override ShaderPass Pass => ShaderPass.COLOR_PRE;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
		
		public class VertexShaderOverlay : ShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public override string Name => "lighting_gouraud";
			public override string Path => "Overlays.Lighting.Gouraud";
			
			public override ShaderType Type => ShaderType.VertexShader;
			public override ShaderPass Pass => ShaderPass.RETURN;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
	}
}