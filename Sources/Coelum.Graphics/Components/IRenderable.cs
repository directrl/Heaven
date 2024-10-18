using System.Numerics;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Components {
	
	public interface IRenderable : INodeComponent {
		
		public GL GL { get; }
		
		public Model Model { get; }
		public Matrix4x4 ModelMatrix { get; }

		public void Render();
	}
}