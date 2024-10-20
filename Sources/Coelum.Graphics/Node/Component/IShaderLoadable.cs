using Coelum.Graphics.OpenGL;
using Coelum.Node;

namespace Coelum.Graphics.Node.Component {

	public interface IShaderLoadable : INodeComponent {

		public void Load(ShaderProgram shader);
	}
}