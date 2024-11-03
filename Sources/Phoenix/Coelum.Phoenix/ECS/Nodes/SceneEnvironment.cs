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
			shader.SetUniform("scene_env.ambient_light", AmbientLight.ToVector3());
		}

		public enum ShadingType {
			
			Phong,
			Gouraud
		}
		
		public static readonly IShaderOverlay[] OVERLAYS = {
			VertexShaderOverlay.OVERLAY,
			FragmentShaderOverlay.OVERLAY
		};
		
		public class VertexShaderOverlay : IShaderOverlay, ILazySingleton<VertexShaderOverlay> {
			
			public static VertexShaderOverlay OVERLAY
				=> ILazySingleton<VertexShaderOverlay>._instance.Value;

			public string Name => "scene_environment";
			public string Path => "Overlays.SceneEnvironment";
			public bool HasHeader => true;
			public bool HasCall => false;
			public ShaderType Type => ShaderType.VertexShader;
			public ShaderPass Pass => ShaderPass.POSITION_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
		
		public class FragmentShaderOverlay : IShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public string Name => "scene_environment";
			public string Path => "Overlays.SceneEnvironment";
			public bool HasHeader => true;
			public bool HasCall => false;
			public ShaderType Type => ShaderType.FragmentShader;
			public ShaderPass Pass => ShaderPass.COLOR_PRE;
			public ResourceManager ResourceManager => Module.RESOURCES;

			public void Load(ShaderProgram shader) { }
		}
	}
}