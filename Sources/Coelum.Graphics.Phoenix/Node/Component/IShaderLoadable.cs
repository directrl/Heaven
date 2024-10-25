using Coelum.Graphics.Phoenix.OpenGL;
using Coelum.Node;

namespace Coelum.Graphics.Phoenix.Node.Component {

	public interface IShaderLoadable : INodeComponent {

		[Obsolete]
		public void Load(ShaderProgram shader);
	}
}