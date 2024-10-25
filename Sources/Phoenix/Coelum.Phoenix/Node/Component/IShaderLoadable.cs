using Coelum.Node;
using Coelum.Phoenix.OpenGL;

namespace Coelum.Phoenix.Node.Component {

	public interface IShaderLoadable : INodeComponent {

		[Obsolete]
		public void Load(ShaderProgram shader);
	}
}