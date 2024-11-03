using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Lighting {
	
	public static class GouraudShading {

		public static readonly IShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY,
			VertexShaderOverlay.OVERLAY
		};
		
		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "lighting_gouraud";
			public string Path => "Overlays.Lighting.Gouraud";
			public bool HasHeader => true;
			public bool HasCall => true;
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
		
		public class VertexShaderOverlay : IShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public string Name => "lighting_gouraud";
			public string Path => "Overlays.Lighting.Gouraud";
			public bool HasHeader => true;
			public bool HasCall => true;
			public ShaderType Type => ShaderType.VertexShader;
			public ShaderPass Pass => ShaderPass.RETURN;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
	}
}