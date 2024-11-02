using System.Drawing;
using Coelum.ECS;
using Coelum.ECS.Tags;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.ECS.Nodes {
	
	public class SceneEnvironment : Node {

		public Color AmbientLight { get; set; }
		
		public SceneEnvironment() {
			AddComponent(new Singleton());
		}

		public void Load(ShaderProgram shader) {
			shader.SetUniform("scene_env.ambient_light", AmbientLight.ToVector4());
		}
		
		public static readonly IShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};

		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "lighting";
			public string Path => "Overlays.Lighting";
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
	}
}