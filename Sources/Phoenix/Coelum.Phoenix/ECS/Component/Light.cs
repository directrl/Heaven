using System.Drawing;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public interface Light : INodeComponent {
		
		public const int LIGHT_DIRECTIONAL = 0;
		public const int LIGHT_POINT = 1;
		public const int LIGHT_SPOT = 2;
		
		public Color Ambient { get; set; }
		public Color Diffuse { get; set; }
		public Color Specular { get; set; }

		public float SpecularStrength { get; set; }

		public void Load(ShaderProgram shader);
	}
}