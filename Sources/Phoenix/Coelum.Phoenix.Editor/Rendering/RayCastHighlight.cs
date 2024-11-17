using System.Drawing;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;
using Coelum.Resources;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Editor.Rendering {
	
	public class RayCastHighlight {
		
		public static readonly ShaderOverlay[] OVERLAYS = {
			FragmentShaderOverlay.OVERLAY
		};

		public class FragmentShaderOverlay : ShaderOverlay, ILazySingleton<FragmentShaderOverlay> {
			
			public static FragmentShaderOverlay OVERLAY
				=> ILazySingleton<FragmentShaderOverlay>._instance.Value;

			public override string Name => "raycast_highlight";
			public override string Path => "Overlays.RayCastHighlight";
			
			public override ShaderType Type => ShaderType.FragmentShader;
			public override ShaderPass Pass => ShaderPass.COLOR_POST;

			public override ResourceManager ResourceManager => EditorApplication.EditorResources;

			public Color HighlightColor { get; set; } = Color.FromArgb(8, 8, 8);

			public override void Include(ShaderProgram shader) {
				var vecColor = HighlightColor.ToVector4();
				shader.Define("RCH_HIGHLIGHT_COLOR", $"vec4({vecColor.X}, {vecColor.Y}, {vecColor.Z}, {vecColor.W})");
			}
		}
	}
}