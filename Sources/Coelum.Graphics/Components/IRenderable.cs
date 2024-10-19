using System.Numerics;
using Coelum.Graphics.OpenGL;
using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Graphics.Components {
	
	public interface IRenderable : INodeComponent {
		
		public GL GL { get; }
		
		public void Render();
	}
}