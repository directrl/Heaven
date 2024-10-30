using Coelum.Node;
using Silk.NET.OpenGL;

namespace Coelum.Phoenix.Node.Component {
	
	public abstract class Renderable : NodeComponent {
		
		public Renderable() : base(null) { }
		
		public abstract void Render();
	}
}