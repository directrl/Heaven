using System.Drawing;
using Coelum.ECS;
using Coelum.LanguageExtensions;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.ECS.Component {
	
	public interface Light : INodeComponent {
		
		public Color Diffuse { get; set; }
		public Color Specular { get; set; }

		public void Load(ShaderProgram shader, string target);
	}
}