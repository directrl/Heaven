using System.Drawing;
using Coelum.ECS;
using Coelum.ECS.Tags;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class SceneEnvironment : Node {

		public Color AmbientColor { get; set; }
		
		public SceneEnvironment() {
			AddComponent(new Singleton());
		}

		public void Load(ShaderProgram shader) {
			shader.SetUniform("scene_env.ambient_color", AmbientColor.ToVector4());
		}
		
		public static readonly ShaderOverlay[] OVERLAYS = {
			VertexShaderOverlay.OVERLAY,
			FragmentShaderOverlay.OVERLAY
		};
		
		public class VertexShaderOverlay : ShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public override string Name => "scene_environment";
			public override string Path => "Overlays.SceneEnvironment";
			
			public override ShaderType Type => ShaderType.VertexShader;
			public override ShaderPass Pass => ShaderPass.POSITION_PRE;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
		
		public class FragmentShaderOverlay : ShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public override string Name => "scene_environment";
			public override string Path => "Overlays.SceneEnvironment";
			
			public override ShaderType Type => ShaderType.FragmentShader;
			public override ShaderPass Pass => ShaderPass.COLOR_PRE_STAGE2;
			
			public override ResourceManager ResourceManager => Module.RESOURCES;
		}
	}
}