using System.Drawing;
using Coelum.ECS;
using Coelum.Phoenix.OpenGL.UBO;

namespace Coelum.Phoenix.ECS.Components {
	
	public interface Light : INodeComponent {
		
		public Color Diffuse { get; set; }
		public Color Specular { get; set; }

		public void Load(Lights ubo, int index);
	}
}